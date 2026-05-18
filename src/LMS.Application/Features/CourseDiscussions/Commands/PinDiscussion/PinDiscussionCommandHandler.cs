using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.CourseDiscussions.Commands.PinDiscussion;

public class PinDiscussionCommandHandler : IRequestHandler<PinDiscussionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;

    public PinDiscussionCommandHandler(IUnitOfWork unitOfWork, IUserContext user)
    {
        _unitOfWork = unitOfWork;
        _user = user;
    }

    public async Task<Result> Handle(PinDiscussionCommand request, CancellationToken cancellationToken)
    {
        var userid = _user.GetCurrentUser().Id;
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return DomainErrors.Course.NotFound(request.CourseId);
        }
        if (course.InstructorId != userid)
        {
            return DomainErrors.Course.NotOwned;
        }
        var discussion = await _unitOfWork.Discussions.GetByIdAsync(request.DiscussionId);
        if (discussion == null)
        {
            return DomainErrors.Discussion.NotFound(request.DiscussionId);
        }
        if (discussion.IsPinned == true)
        {
            return DomainErrors.Discussion.AlreadyPinned;
        }
        discussion.IsPinned = true;
        discussion.PinnedAt = DateTime.UtcNow;
        _unitOfWork.Discussions.Update(discussion);
        await _unitOfWork.CommitAsync();
        return Result.Success();

    }
}
