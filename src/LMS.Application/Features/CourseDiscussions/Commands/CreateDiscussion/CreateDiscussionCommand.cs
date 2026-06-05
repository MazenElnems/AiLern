using LMS.Application.Common.Results;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.CourseDiscussions.Commands.CourseDiscussion;

public class CreateDiscussionCommand : IRequest<Result>
{
    [JsonIgnore]
    public int CourseId { get; set; }
    public string Title { get; set; } 
    public string Content { get; set; } 
}
