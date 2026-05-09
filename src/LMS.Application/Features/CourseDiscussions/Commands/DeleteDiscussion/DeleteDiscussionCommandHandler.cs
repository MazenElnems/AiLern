using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.CourseDiscussions.Commands.DeleteDiscussion;

public class DeleteDiscussionCommandHandler : IRequestHandler<DeleteDiscussionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;

    public DeleteDiscussionCommandHandler(IUnitOfWork unitOfWork, IUserContext user)
    {
        _unitOfWork = unitOfWork;
        _user = user;
    }

    public async Task<Result> Handle(DeleteDiscussionCommand request, CancellationToken cancellationToken)
    {
        var user = _user.GetCurrentUser();
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return DomainErrors.Course.NotFound(request.CourseId);
        }
        if (user.IsInRole("Instructor") && course.InstructorId != user.Id)
        {
            return DomainErrors.Course.NotOwned;
        }
        var discussion = await _unitOfWork.Discussions.GetByIdAsync(request.DiscussionId);
        if (discussion == null)
        {
            return DomainErrors.Discussion.NotFound(request.DiscussionId);
        }
        if (user.IsInRole("Student") && discussion.StudentId != user.Id)
        {
            return DomainErrors.Discussion.NotOwned;
        }
        _unitOfWork.Discussions.Delete(discussion);
        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
