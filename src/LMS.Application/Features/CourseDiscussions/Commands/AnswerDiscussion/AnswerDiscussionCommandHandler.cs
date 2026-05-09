using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.CourseDiscussions.Commands.AnswerDiscussion;

public class AnswerDiscussionCommandHandler : IRequestHandler<AnswerDiscussionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;

    public AnswerDiscussionCommandHandler(IUnitOfWork unitOfWork, IUserContext user)
    {
        _unitOfWork = unitOfWork;
        _user = user;
    }

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
        await _unitOfWork.CommitAsync();
        return Result.Success("Discussion answered successfully.");
    }
}
