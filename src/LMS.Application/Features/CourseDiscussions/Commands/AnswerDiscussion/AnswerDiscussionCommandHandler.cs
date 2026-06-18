using LMS.Application.Common.Results;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Notification;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.CourseDiscussions.Commands.AnswerDiscussion;

public class AnswerDiscussionCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext user,
    INotificationService notificationService) : IRequestHandler<AnswerDiscussionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserContext _user = user;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result> Handle(AnswerDiscussionCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.GetCurrentUser().Id;
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return DomainErrors.Course.NotFound(request.CourseId);
        }
        if (course.InstructorId != userId)
        {
            return DomainErrors.Course.NotOwned;
        }
        var discussion = await _unitOfWork.Discussions.GetByIdAsync(request.DiscussionId);
        if (discussion == null)
        {
            return DomainErrors.Discussion.NotFound(request.DiscussionId);
        }
        discussion.Answer = request.Answer;
        discussion.AnswerAt = DateTime.UtcNow;
        discussion.IsAnswered = true;
        _unitOfWork.Discussions.Update(discussion);
        await _unitOfWork.CommitAsync(cancellationToken);

        await _notificationService.NotifyQuestionRepliedAsync(
            discussion.StudentId,
            request.CourseId,
            "Instructor Replied to Your Question",
            $"Your discussion '{discussion.Title}' has been answered by the instructor."
        );

        return Result.Success("Discussion answered successfully.");
    }
}
