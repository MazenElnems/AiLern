using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.CourseDiscussions.Commands.UnPinDiscussion;

public class UnPinDiscussionCommandHandler : IRequestHandler<UnPinDiscussionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;

    public UnPinDiscussionCommandHandler(IUnitOfWork unitOfWork, IUserContext user)
    {
        _unitOfWork = unitOfWork;
        _user = user;
    }

    public async Task<Result> Handle(UnPinDiscussionCommand request, CancellationToken cancellationToken)
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
        if (discussion.IsPinned == false)
        {
            return DomainErrors.Discussion.NotPinned;
        }
        discussion.IsPinned = false;
        discussion.PinnedAt = null;
        _unitOfWork.Discussions.Update(discussion);
        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
