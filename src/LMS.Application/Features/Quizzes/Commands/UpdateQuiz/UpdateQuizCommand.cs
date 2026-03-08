using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.Requests;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommand : QuizRequest, IRequest<Result<Guid>>
{
    [JsonIgnore]
    public Guid Id { get; set; }
}
