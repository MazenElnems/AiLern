using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.AddBatchQuestions;

public record AddBatchQuestionsByTypeCommand(
    string QuestionType,
    int GeneratedQuestions,
    List<AIQuestionGeneratedResponse> Questions,
    bool Completed,
    Guid QuizId 
) : IRequest<Result>;
