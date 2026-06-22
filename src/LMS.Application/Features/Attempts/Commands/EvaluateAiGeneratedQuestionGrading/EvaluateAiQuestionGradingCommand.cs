using LMS.Application.Common.Results;
using LMS.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Attempts.Commands.EvaluateAiGeneratedQuestionGrading;

public class EvaluateAiQuestionGradingCommand : IRequest<Result>
{
    [JsonIgnore]
    public Guid AttemptId { get; set; }
    [JsonIgnore]
    public Guid QuestionId { get; set; }

    public AccuracyRating AccuracyRating { get; set; }
    public FeedbackThemes FeedbackThemes { get; set; }
    public string? EvaluateComment { get; set; }
}
