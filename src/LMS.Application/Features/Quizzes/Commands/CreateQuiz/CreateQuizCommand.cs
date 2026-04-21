using LMS.Application.Common.Results.Generic;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Quizzes.Commands.CreateQuiz;

public class CreateQuizCommand() : IRequest<Result<Guid>>
{
    [JsonIgnore]
    public int CourseId { get; set; }   
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

