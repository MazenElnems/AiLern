using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace LMS.Infrastructure.Jobs;

public class AIBatchGradingJob(IUnitOfWork unitOfWork, IAIService aiService, ILogger<AIBatchGradingJob> logger, IMapper mapper) 
    : IAIBatchGradingJob
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAIService _aiService = aiService;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AIBatchGradingJob> _logger = logger;

    private const int BatchSize = 10;
    private const int Retries = 3;

    public async Task ExecuteAsync(int courseId, Guid quizId, List<Guid> attemptIds, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start AI Batch Grading Background Job for {@AttemptIds}",
            attemptIds
        );

        var aiQuestionGradingRequest = await _unitOfWork.Questions.Query
            .Where(q => q.QuizId == quizId)
            .ProjectTo<AIQuestionsGrading>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var numberOfBatches = Math.Ceiling(attemptIds.Count / (double)BatchSize);
        int currentBatch = 0;

        while (numberOfBatches > 0)
        {
            numberOfBatches--;
            currentBatch++;

            var attemptBatchIds = attemptIds
                .Skip(BatchSize * (currentBatch - 1))
                .Take(BatchSize);

            var attempts = await _unitOfWork.Attempts.Query
                .Include(a => a.Answers)
                    .ThenInclude(a => a.Option)
                .AsSplitQuery()
                .Where(a => attemptBatchIds.Contains(a.Id))
                .ToListAsync(cancellationToken);

            var batchAnswers = _mapper.Map<List<StudentBatchAnswer>>(attempts);
            var aiGradingRequest = new AIGradingRequest { Questions = aiQuestionGradingRequest, StudentAnswers = batchAnswers };
            
            try
            {
                var result = await _aiService.GradeQuizBatchAsync(aiGradingRequest, cancellationToken);
                await AdjustAIFeedback(courseId, attempts, result, cancellationToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode != HttpStatusCode.BadRequest)
            {
                _logger.LogWarning(
                    ex,
                    "Retries : {Retries} for AI Grading Request", Retries
                );

                int retry = 0;
                bool graded = false;
                while(retry < Retries)
                {
                    retry++;

                    try
                    {
                        var result = await _aiService.GradeQuizBatchAsync(aiGradingRequest, cancellationToken);
                        await AdjustAIFeedback(courseId, attempts, result, cancellationToken);
                        graded = true;
                        break;
                    }
                    catch(HttpRequestException exp)
                    {
                        _logger.LogWarning(exp,
                            "Retry #{Retry} failed",
                            retry + 1
                        );
                    }

                    // BackOff Delay between retries
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2.0, retry + 1)), cancellationToken);
                }

                if (!graded)
                    await UpdateGradingStatus(AIGradingStatus.Failed, attempts, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "AI batch grading failed for batch {Batch} of quiz {QuizId}",
                    currentBatch, quizId
                );

                await UpdateGradingStatus(AIGradingStatus.Failed, attempts, cancellationToken);
            }
        }
    }
    private async Task AdjustAIFeedback(int courseId, List<Attempt> attempts, AIBatchGradingResponse gradingResponse, CancellationToken cancellationToken)
    {
        foreach (var attemptFeedback in gradingResponse.AttemptFeedbacks)
        {
            var attemptToGrade = attempts.First(a => a.Id == attemptFeedback.AttemptId);

            var attemptWeakTopics = attemptFeedback.WeakTopics.Select(t => new WeakTopic
            {
                CourseId = courseId,
                AttemptId = attemptFeedback.AttemptId,
                Topic = t
            });

            await _unitOfWork.WeakTopics.InsertRangeAsync(attemptWeakTopics);

            foreach (var questionFeedback in attemptFeedback.QuestionFeedback)
            {
                var answer = attemptToGrade.Answers.First(a => a.QuestionId == questionFeedback.Id);

                answer.Feedback = questionFeedback.Feedback;
                answer.Mark = questionFeedback.EstimatedScore;
            }

            attemptToGrade.Status = AttemptStatus.Graded;
            attemptToGrade.AIGradingStatus = AIGradingStatus.Graded;
        }

        await _unitOfWork.CommitAsync(cancellationToken);
    }

    private async Task UpdateGradingStatus(AIGradingStatus status, List<Attempt> attempts, CancellationToken cancellationToken)
    {
        foreach (var attempt in attempts)
            attempt.AIGradingStatus = status;

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
