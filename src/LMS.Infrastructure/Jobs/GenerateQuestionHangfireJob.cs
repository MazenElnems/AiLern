using Hangfire;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LMS.Infrastructure.Jobs;

public class GenerateQuestionHangfireJob   // Hangfire Adapter
{
    private readonly IGenerateQuestionsJob _job;

    public GenerateQuestionHangfireJob(IGenerateQuestionsJob job)
    {
        _job = job;
    }

    public Task Execute(Guid jobId,
           Guid quizId,
           int questionsCount,
           Dictionary<QuestionType, int> questionTypeCounts,
           Dictionary<QuestionDifficultyLevels, float> questionDifficultyPercents,
           IJobCancellationToken token,
           string? query = null)
    {
        return _job.ExecuteAsync(jobId,
            quizId,
            questionsCount,
            questionTypeCounts,
            questionDifficultyPercents,
            token.ShutdownToken, query
        );
    }
}
