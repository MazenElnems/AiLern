using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.CourseDiscussions.Commands.DownVoteDiscussion;

public class DownVoteDiscussionCommand : IRequest<Result>
{
    public int CourseId { get; set; }
    public Guid DiscussionId { get; set; }
}
