using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.CourseDiscussion;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.CourseDiscussions.Commands.CourseDiscussion;

public class CreateDiscussionCommandHandler : IRequestHandler<CreateDiscussionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;

    public CreateDiscussionCommandHandler(IUnitOfWork unitOfWork, IUserContext user)
    {
        _unitOfWork = unitOfWork;
        _user = user;
    }

    public async Task<Result> Handle(CreateDiscussionCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.GetCurrentUser().Id;
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return DomainErrors.Course.NotFound(request.CourseId);
        }
        var isEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, userId);
        if (!isEnrolled)
        {
            return DomainErrors.Course.NotEnrolled;
        }
        await _unitOfWork.Discussions.InsertAsync(new Discussion
        {
            CourseId = request.CourseId,
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            StudentId = userId
        });
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }
}
