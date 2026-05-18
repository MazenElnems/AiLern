using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.CourseDiscussions.Commands.VoteDiscussion;

public class UpVoteDiscussionCommand : IRequest<Result>
{
    public int CourseId { get; set; }
    public Guid DiscussionId { get; set; }
}
