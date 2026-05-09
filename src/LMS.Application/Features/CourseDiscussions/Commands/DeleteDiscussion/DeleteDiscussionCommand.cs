using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.CourseDiscussions.Commands.DeleteDiscussion;

public class DeleteDiscussionCommand : IRequest<Result>
{
    public int CourseId { get; set; }
    public Guid DiscussionId { get; set; }
}
