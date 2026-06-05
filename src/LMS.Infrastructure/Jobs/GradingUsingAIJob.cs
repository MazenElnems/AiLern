using LMS.Application.Common.Models.Request;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Jobs;

public class GradingUsingAIJob(IAIService aiService, ILogger<GradingUsingAIJob> logger, IUnitOfWork unitOfWork) : IGradingUsingAIJob
{
    private readonly IAIService _aiService = aiService;
    private readonly ILogger<GradingUsingAIJob> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task ExecuteAsync(Guid quizId, List<Guid> attemptIds, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting AI grading job for quiz {QuizId}, attempts: {AttemptIds}", quizId, attemptIds);

        var questions = await _unitOfWork.Questions.Query
            .Where(q => q.QuizId == quizId)
            .Include(q => q.Options)
            .Include(q => q.Criterias)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var questionsLookup = questions.ToDictionary(q => q.Id);

        foreach (var id in attemptIds)
        {
            try
            {
                var attempt = await _unitOfWork.Attempts.Query
                    .Include(a => a.Answers)
                        .ThenInclude(a => a.Option)
                    .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

                if (attempt is null)
                {
                    _logger.LogWarning("Attempt {AttemptId} not found, skipping.", id);
                    continue;
                }

                var studentAnswers = attempt.Answers.Select(a =>
                {
                    questionsLookup.TryGetValue(a.QuestionId, out var question);

                    return new AIAnswerGradingRequest
                    {
                        Id = a.QuestionId,
                        QuestionText = question?.QuestionText ?? string.Empty,
                        Mark = question?.Mark ?? 0,
                        Type = question?.Type ?? QuestionType.Written,
                        Options = question?.Options?.Select(o => o.OptionText).ToList(),
                        GradingCriteria = question?.Criterias?.Select(c => new AIGradingCriteriaRequest
                        {
                            Criteria = c.Criteria,
                            Weight = c.Mark,
                        }).ToList() ?? new(),
                        StudentAnswer = question?.Type == QuestionType.Written
                            ? a.WrittenAnswer
                            : a.Option?.OptionText,
                        QuestionAnswer = question?.AIGradingReferenceAnswer
                    };
                }).ToList();

                var gradingRequest = new AIQuizSubmissionGradingRequest
                {
                    AttemptId = id,
                    StudentAnswers = studentAnswers
                };

                var result = await _aiService.GradeAsync(gradingRequest, cancellationToken);

                foreach (var answerFeedback in result.AnswersFeedback)
                {
                    var answer = attempt.Answers.FirstOrDefault(a => a.QuestionId == answerFeedback.Id);
                    if (answer is not null)
                    {
                        answer.Feedback = answerFeedback.Feedback;
                        answer.Mark = answerFeedback.EstinatedScore;
                    }
                }

                attempt.WeakTopics = result.WeakTopics ?? new();
                attempt.Status = AttemptStatus.AIGraded;

                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("Successfully graded attempt {AttemptId}.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to grade attempt {AttemptId}.", id);
            }
        }
    }
}
