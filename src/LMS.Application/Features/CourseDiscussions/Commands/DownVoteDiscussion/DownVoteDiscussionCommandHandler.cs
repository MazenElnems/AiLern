using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.CourseDiscussion;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.CourseDiscussions.Commands.DownVoteDiscussion;

public class DownVoteDiscussionCommandHandler : IRequestHandler<DownVoteDiscussionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;

    public DownVoteDiscussionCommandHandler(IUnitOfWork unitOfWork, IUserContext user)
    {
        _unitOfWork = unitOfWork;
        _user = user;
    }

    public async Task<Result> Handle(DownVoteDiscussionCommand request, CancellationToken cancellationToken)
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
        var discussion = await _unitOfWork.Discussions.GetByIdAsync(request.DiscussionId);
        if (discussion == null)
        {
            return DomainErrors.Discussion.NotFound(request.DiscussionId);
        }
        var discussionVote = await _unitOfWork.DiscussionVotes.Query.AsNoTracking().Where(d => d.DiscussionId == request.DiscussionId && d.StudentId == userId).FirstOrDefaultAsync();
        if (discussionVote == null)
        {
            return DomainErrors.Discussion.NotVoted;
        }
         _unitOfWork.DiscussionVotes.Delete(discussionVote);


        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
