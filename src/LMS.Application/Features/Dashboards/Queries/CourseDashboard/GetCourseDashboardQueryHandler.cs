using LMS.Application.Features.Dashboards.Shared.DTO;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Errors;
using MediatR;
using LMS.Application.Contracts.UnitOfWork;

namespace LMS.Application.Features.Dashboards.Queries.CourseDashboard;

public class GetCourseDashboardQueryHandler : IRequestHandler<GetCourseDashboardQuery, Result<CourseDashboardDto>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public GetCourseDashboardQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CourseDashboardDto>> Handle(GetCourseDashboardQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);

        if (course == null)
            return DomainErrors.Course.NotFound(request.CourseId);

        if (course.InstructorId != user.Id)
            return DomainErrors.Course.NotOwned;

        var totalStudents = await _unitOfWork.Enrollments.CountAsync(e => e.CourseId == request.CourseId);
        var totalQuizzes = await _unitOfWork.Quizzes.CountAsync(q => q.CourseId == request.CourseId);
        var totalAssignments = await _unitOfWork.Assignments.CountAsync(a => a.CourseId == request.CourseId);

        var quizStatistics = await _unitOfWork.Quizzes.GetQuizStatisticsForCourseAsync(request.CourseId);

        return new CourseDashboardDto
        {
            TotalQuizzes = totalQuizzes,
            TotalEnrolledStudents = totalStudents,
            TotalAssignments = totalAssignments,
            QuizStatistics = quizStatistics
        };
    }
}
