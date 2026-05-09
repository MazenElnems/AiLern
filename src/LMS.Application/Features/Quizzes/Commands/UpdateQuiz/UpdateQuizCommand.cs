using LMS.Application.Common.Results;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommand : IRequest<Result>
{
    [JsonIgnore]
    public Guid QuizId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableUntil { get; set; }
    public int AttemptTimeLimit { get; set; }
    public int MaximumAttempts { get; set; }
    public bool ShowResultOnClose { get; set; }
    public bool ShuffleQuestions { get; set; }
    public bool ShuffleOptions { get; set; }
}
