using LMS.Application.Common.Results;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.CourseDiscussions.Commands.AnswerDiscussion;

public class AnswerDiscussionCommand : IRequest<Result>
{
    [JsonIgnore]
    public Guid DiscussionId { get; set; }
    [JsonIgnore]
    public int CourseId { get; set; }
    public string Answer { get; set; }
}
