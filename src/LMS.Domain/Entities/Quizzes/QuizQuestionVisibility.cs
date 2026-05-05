using System.Linq.Expressions;

namespace LMS.Domain.Entities.Quizzes;

/// <summary>
/// Questions shown in live quiz content: manual questions always; AI questions only after acceptance.
/// </summary>
public static class QuizQuestionVisibility
{
    public static readonly Expression<Func<Question, bool>> IsLive = q =>
        !q.IsAIGenerated || q.IsAccepted == true;

    public static readonly Expression<Func<Question, bool>> IsPendingAi = q =>
        q.IsAIGenerated && q.IsAccepted != true;

    public static bool IsLiveQuestion(Question q) =>
        !q.IsAIGenerated || q.IsAccepted == true;
}
