using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Assignments.DTO;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommand : IRequest<Result<GetAllQuizDto>>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableUntil { get; set; }
    public bool ShowCorrectAnswersAfterClose { get; set; }
    public bool IsPublished { get; set; }
    public bool ShuffleQuestions { get; set; }
    public bool ShuffleOptions { get; set; }
    public int MaximumAttempts { get; set; }
    public int TotalPoints { get; set; }
    


}

