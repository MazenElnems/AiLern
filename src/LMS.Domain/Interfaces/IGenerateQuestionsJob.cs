using LMS.Domain.Enums;

namespace LMS.Domain.Interfaces;

public interface IGenerateQuestionsJob
{
    Task ExecuteAsync(Guid jobId,
        Guid quizId,
        int questionsCount,
        Dictionary<QuestionType, int> questionTypeCounts,
        Dictionary<QuestionDifficultyLevels, float> questionDifficultyPercents,
        CancellationToken cancellationToken,
        string? query = null);
}
