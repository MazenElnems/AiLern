using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LMS.Application.Features.Quizzes.Commands.QenerateQuestionsUsingAI;

public class GenerateQuestionsCommandHandler : IRequestHandler<GenerateQuestionsCommand, Result<Guid>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBackgroundJobService _backgroundService;

    public GenerateQuestionsCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork, IBackgroundJobService backgroundService)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _backgroundService = backgroundService;
    }

    public async Task<Result<Guid>> Handle(GenerateQuestionsCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;
        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId, includeProperties: [nameof(Quiz.Course)]);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);
       
        var course = quiz.Course;


        if (course.InstructorId != userId)
            return DomainErrors.Quiz.NotOwned;

        var newJob = new AIQuestionGenerationJob
        {
            QuizId = request.QuizId,
            CreatedAt = DateTime.UtcNow,
            Status = AIJobStatus.Pending,
            CompletedAt = null,
            Error = null
        };

        await _unitOfWork.QuestionGenerationJobs.InsertAsync(newJob);
        await _unitOfWork.CommitAsync();

        var hangfireJobId = _backgroundService.Enqueue<GenerateQuestionHangfireJob>(x => x.ExecuteAsync(newJob.Id,
            request.QuizId,
            request.MaterialIds,
            request.UploadedFiles,
            request.QuestionsCount,
            request.QuestionTypeCounts,
            request.QuestionDifficultyPercents,
            request.Query));

        newJob.HangfireJobId = hangfireJobId;

        await _unitOfWork.CommitAsync();


        return Result<Guid>.Success(newJob.Id, "Quiz generation with AI has started.");

    }
}
