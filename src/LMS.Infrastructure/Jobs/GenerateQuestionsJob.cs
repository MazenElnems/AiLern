using AutoMapper;
using LMS.Application.Common.Models.Request;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Jobs;

public class GenerateQuestionsJob : IGenerateQuestionsJob
{
    private readonly IAIService _service;
    private readonly IWasabiService _wasabiService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GenerateQuestionsJob> _logger;

    public GenerateQuestionsJob(IAIService service, IWasabiService wasabiService, IUnitOfWork unitOfWork, IMapper mapper, ILogger<GenerateQuestionsJob> logger)
    {
        _service = service;
        _wasabiService = wasabiService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid jobId,
        Guid quizId,
        int questionsCount,
        Dictionary<QuestionType, int> questionTypeCounts,
        Dictionary<QuestionDifficultyLevels, float> questionDifficultyPercents,
        CancellationToken cancellationToken,
        string? query = null)
    {
        var job = default(AIQuestionGenerationJob);

        try
        {
            _logger.LogInformation("Start Question Generation Job: {Job}", jobId);

            cancellationToken.ThrowIfCancellationRequested();

            job = await _unitOfWork.QuestionGenerationJobs.GetByIdAsync(jobId)
                ?? throw new ArgumentException("Invalid Job Id");

            job.Status = AIJobStatus.InProgress;
            job.Error = null;
            job.CompletedAt = null;
            await _unitOfWork.CommitAsync();

            var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == quizId,
                includeProperties: [nameof(Quiz.QuestionGenerationFiles)]);

            var questionGenerationFiles = quiz.QuestionGenerationFiles;

            var notUploadedFiles = questionGenerationFiles.Where(f => !f.HasUploadedToAIService);

            var streamsByKey = await _wasabiService
                .GetFileStreamAsync(notUploadedFiles.Select(f => f.StoragePath).ToList());

            foreach(var q in notUploadedFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var stream = streamsByKey[q.StoragePath];
                var response = await _service.UploadFileAsync(q.Id.ToString(), q.FileName, stream, cancellationToken);

                if(!response.Status.Equals("ok", StringComparison.OrdinalIgnoreCase))
                { 
                    job.Status = AIJobStatus.Failed;
                    job.Error = $"Failed to upload file {q.FileName} to AI service.";
                    job.CompletedAt = DateTime.UtcNow;
                    await _unitOfWork.CommitAsync();
                    _logger.LogError("Question Generation Job: {Job} Was Failed {Message}", jobId, job.Error);
                    return;
                }
            }

            foreach(var file in notUploadedFiles)
            {
                file.HasUploadedToAIService = true;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var request = new AIQuizGenerationRequest
            {
                ProjectIds = questionGenerationFiles.Select(f => f.Id.ToString()).ToArray(),
                QuestionsNumber = questionsCount,
                QuestionsTypes = questionTypeCounts,
                DifficultyLevels = questionDifficultyPercents,
                Query = query
            };

            cancellationToken.ThrowIfCancellationRequested();

            var result = await _service.GenerateQuestionsAsync(request, cancellationToken);

            if(!result.Status.Equals("ok", StringComparison.OrdinalIgnoreCase))
            {
                job.Status = AIJobStatus.Failed;
                job.Error = $"Failed to generate questions. Message: {result.Message}";
                job.CompletedAt = DateTime.UtcNow;
                await _unitOfWork.CommitAsync();
                _logger.LogError("Question Generation Job: {Job} Was Failed {Message}", jobId, result.Message);
                return;
            }

            var questions = _mapper.Map<List<Question>>(result.Questions);

            quiz.Questions.AddRange(questions);

            cancellationToken.ThrowIfCancellationRequested();

            job.Status = AIJobStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();
        }
        catch(OperationCanceledException ex)
        {
            _logger.LogError(ex, "Question Generation Job: {Job} Was Canceled", jobId);
            if(job != null)
            {
                job.Status = AIJobStatus.Canceled;
                await _unitOfWork.CommitAsync();
            }
        }
        catch(ArgumentException ex)
        {
            _logger.LogError(ex, "{Job} not found", jobId);
        }
        catch(Exception ex)
        {
            if (job != null)
            {
                job.Status = AIJobStatus.Failed;
                job.Error = "An Exception Was Thrown While Run Question Generation Job";
                await _unitOfWork.CommitAsync();
            }
            _logger.LogError(ex, "An Exception Was Thrown While Run Question Generation Job: {Job}", jobId);
        }
    }
}
