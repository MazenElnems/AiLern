using LMS.Application.Common.Results;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.CourseDiscussions.Commands.UpdateDiscussion;

public class UpdateDiscussionCommand : IRequest<Result>
{
    [JsonIgnore]
    public int CourseId { get; set; }
    [JsonIgnore]
    public Guid DiscussionId { get; set; }
    public string Title { get; set; } 
    public string Content { get; set; }
}
