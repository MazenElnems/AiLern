using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.CourseDiscussions.Shared.DTO;
using LMS.Domain.Constants;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.CourseDiscussions.Queries.GetDiscussions;

public class GetDiscussionsCommandHandler : IRequestHandler<GetDiscussionsCommand, Result<List<DiscussionDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;
    private readonly IBunnyUrlSigner _bunny;

    public GetDiscussionsCommandHandler(IUnitOfWork unitOfWork, IUserContext user, IBunnyUrlSigner bunny)
    {
        _unitOfWork = unitOfWork;
        _user = user;
        _bunny = bunny;
    }

    public async Task<Result<List<DiscussionDto>>> Handle(GetDiscussionsCommand request, CancellationToken cancellationToken)
    {
        var user = _user.GetCurrentUser();
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return DomainErrors.Course.NotFound(request.CourseId);
        }
        if (user.IsInRole(UserRoles.Instructor) && course.InstructorId != user.Id)
        {
            return DomainErrors.Course.NotOwned;
        }
        if (user.IsInRole(UserRoles.Student))
        {
            var isenrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, user.Id);
            if (isenrolled == false)
            {
                return DomainErrors.Course.NotEnrolled;
            }
        }
        var dto = await _unitOfWork.Discussions.Query.AsNoTracking().Where(d => d.CourseId == request.CourseId)
            .Select(d => new DiscussionDto
            {
                Id = d.Id,
                Title = d.Title,
                Question = d.Content,
                CreatedAt = d.CreatedAt,
                Answer = d.Answer,
                AnswerAt = d.AnswerAt,
                IsPinned = d.IsPinned,
                PinnedAt = d.PinnedAt,
                StudentName = d.Student.FullName,
                InstructorName = d.IsAnswered ? d.Course.Instructor.FullName : null,
                InstructorAvatar = d.IsAnswered && !string.IsNullOrEmpty(d.Course.Instructor.ImageStoragePath) ? _bunny.GetUrl(d.Course.Instructor.ImageStoragePath) : null,
                VotesCount = d.Votes.Count(),
                StudentAvatar = !string.IsNullOrEmpty(d.Student.ImageStoragePath) ? _bunny.GetUrl(d.Student.ImageStoragePath) : null,
                IsUpVotedByCurrentUser = d.Votes.Any(v => v.StudentId == user.Id)
            }).ToListAsync(cancellationToken);

        return Result<List<DiscussionDto>>.Success(dto);
    }
}
