using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.QenerateQuestionsUsingAI;

public class GenerateQuestionsCommandHandler : IRequestHandler<GenerateQuestionsCommand, Result<Guid>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWasabiService _wasabiService;
    private readonly IBackgroundJobService _backgroundService;

    public GenerateQuestionsCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork, IBackgroundJobService backgroundService, IWasabiService wasabiService)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _backgroundService = backgroundService;
        _wasabiService = wasabiService;
    }

    public async Task<Result<Guid>> Handle(GenerateQuestionsCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;
        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);
        if (quiz is null)
            return Result<Guid>.Failure(DomainErrors.Quiz.NotFound(request.QuizId));
        if (quiz.Course.InstructorId != userId)
            return Result<Guid>.Failure(DomainErrors.Quiz.NotOwned);

        var course = quiz.Course;

        var questionGenerationFiles = (await _unitOfWork.QuestionGenerationFiles
            .FilterAsync(f => f.QuizId == request.QuizId)).ToList();

        var addedFileIds = request.FileIds.Except(questionGenerationFiles.Select(f => f.Id)).ToList();
        var removedFileIds = questionGenerationFiles.Select(f => f.Id).Except(request.FileIds).ToList();

        var sections = await _unitOfWork.Sections.FilterAsync(s => s.CourseId == course.Id && s.MaterialFiles.Any(f => addedFileIds.Contains(f.Id)));
        var newAddedMaterialFiles = sections.SelectMany(s => s.MaterialFiles);

        questionGenerationFiles.RemoveAll(f => removedFileIds.Contains(f.Id));
        questionGenerationFiles.AddRange(newAddedMaterialFiles.Select(f => new QuestionGenerationFiles { Id = f.Id, QuizId = request.QuizId, IsCourseMaterial = true, StoragePath = f.StoragePath, FileName = f.FileName, HasUploadedToAIService = false }));

        if (request.NewUploadedFiles.Any())
        {
            var newUploadedFileStreams = request.NewUploadedFiles.Select(f => f.OpenReadStream()).ToList();
            var newUploadedFileKeys = request.NewUploadedFiles.Select(f =>
            {
                var filePath = $"ailern-storage2/quiz-{request.QuizId}/uploaded/{Guid.NewGuid()}-{f.FileName}";
                return filePath;
            }).ToList();

            await _wasabiService.UploadFilesAsync(newUploadedFileStreams, newUploadedFileKeys);

            questionGenerationFiles.AddRange(newUploadedFileKeys.Select(f => new QuestionGenerationFiles
            {
                Id = Guid.NewGuid(),
                FileName = f.Split('/').Last().Split('-').Last(),
                StoragePath = f,
                IsCourseMaterial = false,
                HasUploadedToAIService = false,
            }));
        }

        var newJob = new AIQuestionGenerationJob
        {
            QuizId = request.QuizId,
            CreatedAt = DateTime.UtcNow,
            Status = AIJobStatus.Pending,
        };

        await _unitOfWork.QuestionGenerationJobs.InsertAsync(newJob);
        await _unitOfWork.CommitAsync();

        var hangfireJobId = _backgroundService
            .Enqueue<IGenerateQuestionsJob>(_job => _job.ExecuteAsync(
                newJob.Id,
                request.QuizId,
                request.QuestionsCount,
                request.QuestionTypeCounts,
                request.QuestionDifficultyPercents,
                CancellationToken.None,
                request.Query
            ));

        newJob.HangfireJobId = hangfireJobId;

        await _unitOfWork.CommitAsync();

        return Result<Guid>.Success(newJob.Id, "Quiz generation with AI has started.");
    }
}
