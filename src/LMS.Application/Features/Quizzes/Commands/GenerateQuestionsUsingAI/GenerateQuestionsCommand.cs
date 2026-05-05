using LMS.Application.Common.Results;
using LMS.Domain.Enums;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.QenerateQuestionsUsingAI;

public record GenerateQuestionsCommand(
    Guid QuizId,
    List<Guid> FileIds,
    List<string> Topics,
    int QuestionsCount,
    Dictionary<QuestionType, int> QuestionTypeCounts,
    Dictionary<QuestionDifficultyLevels, float> QuestionDifficultyPercents,
    string? Query
) : IRequest<Result>;
