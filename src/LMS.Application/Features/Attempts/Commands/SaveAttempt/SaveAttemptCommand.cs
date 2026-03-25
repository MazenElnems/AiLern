using LMS.Application.Common.Results;
using LMS.Application.Features.Attempts.Shared.Requests;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Attempts.Commands.SaveAttempt;

public class SaveAttemptCommand : IRequest<Result>
{
    [JsonIgnore]
    public Guid AttemptId { get; set; }
    public List<SaveAttemptAnswerRequest> Answers { get; set; } = new();
}
