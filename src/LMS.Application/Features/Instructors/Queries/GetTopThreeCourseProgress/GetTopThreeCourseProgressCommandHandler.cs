using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Instructors.Shared.DTO;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Instructors.Queries.GetTopThreeCourseProgress;

public class GetTopThreeCourseProgressQueryHandler : IRequestHandler<GetTopThreeCourseProgressQuery, Result<List<TopThreeCourseProgressDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;
    public GetTopThreeCourseProgressQueryHandler(IUnitOfWork unitOfWork, IUserContext user)
    {
        _unitOfWork = unitOfWork;
        _user = user;
    }

    public async Task<Result<List<TopThreeCourseProgressDto>>> Handle(GetTopThreeCourseProgressQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.GetCurrentUser().Id;
        var course = await _unitOfWork.Courses.Query
            .AsNoTracking()
            .Where(c => c.InstructorId == userId)
            .Select(c => new TopThreeCourseProgressDto
            {
                CourseId = c.Id,
                CourseName = c.Name,
                ProgressPercentage = c.Sections.Count() == 0 || c.Enrollments.Count() == 0 ? 0 :
                    Math.Round((_unitOfWork.SectionProgress.Query
                        .Where(s => s.Section.CourseId == c.Id).Count() /
                    (double)(c.Sections.Count() * c.Enrollments.Count()) * 100), 2),
                StudentsCount = c.Enrollments.Count,
                QuizzesCount = c.Quizzes.Count
            })
            .OrderByDescending(c => c.ProgressPercentage)
            .Take(3)
            .ToListAsync();
        return course;
    }
}
