using LMS.Application.Common.Results;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.CourseDiscussions.Commands.UnPinDiscussion;

public class UnPinDiscussionCommand : IRequest<Result>
{
    [JsonIgnore]
    public int CourseId { get; set; }
    [JsonIgnore]
    public Guid DiscussionId { get; set; }
}
