using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Admin.Shared.DTO;
using LMS.Domain.Constants;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace LMS.Application.Features.Admin.Queries.GetAdminDashboard;

public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, Result<AdminDashboardDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAdminDashboardQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AdminDashboardDto>> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        var usersCount = await _unitOfWork.Users.Query.AsNoTracking()
            .GroupBy(u => u.Role)
            .Select(g => new { Role = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x=>x.Role,v=>v.Count);

        var totalCourses = await _unitOfWork.Courses.Query
            .AsNoTracking()
            .CountAsync();

        var totalEnrollments = await _unitOfWork.Enrollments.Query
            .AsNoTracking()
            .CountAsync();

        var currentYear = DateTime.Today.Year;

        var userGrowthPerMonths = await _unitOfWork.Users.Query.Where(u=>u.CreatedAt.Year == currentYear)
            .GroupBy(u => u.CreatedAt.Month)
            .Select(g => new UserGrowthPerMonthDto
            {
                Month = new DateTime(1, g.Key, 1)
                        .ToString("MMM", CultureInfo.InvariantCulture),

                StudentsCount = g.Where(u=>u.Role==UserRoles.Student).Count(),

                InstructorsCount = g.Where(u=>u.Role==UserRoles.Instructor).Count()
            }).ToListAsync();

        var top3Courses = await _unitOfWork.Courses.Query.AsNoTracking()
            .Select(c => new TopCourseDto
            {
                CourseId = c.Id,
                CourseName = c.Name,
                InstructorName = c.Instructor.FullName,
                TotalStudents = c.Enrollments.Count()
            })
            .OrderByDescending(c => c.TotalStudents)
            .Take(3).ToListAsync();



        for (int i = 1; i < userGrowthPerMonths.Count; i++)
        {
            userGrowthPerMonths[i].InstructorsGrowthPercentage =
                userGrowthPerMonths[i - 1].InstructorsCount == 0
                    ? (userGrowthPerMonths[i].InstructorsCount == 0 ? 0.0 : 100.0)
                    : ((double)(userGrowthPerMonths[i].InstructorsCount - userGrowthPerMonths[i - 1].InstructorsCount)
                       / userGrowthPerMonths[i - 1].InstructorsCount) * 100.0;

            userGrowthPerMonths[i].StudentsGrowthPercentage =
                userGrowthPerMonths[i - 1].StudentsCount == 0
                    ? (userGrowthPerMonths[i].StudentsCount == 0 ? 0.0 : 100.0)
                    : ((double)(userGrowthPerMonths[i].StudentsCount - userGrowthPerMonths[i - 1].StudentsCount)
                       / userGrowthPerMonths[i - 1].StudentsCount) * 100.0;
        }

        var result = new AdminDashboardDto
        {
            TotalStudents = usersCount[UserRoles.Student],
            TotalInstructors = usersCount[UserRoles.Instructor],
            TotalAdmins = usersCount[UserRoles.Admin],
            TotalCourses = totalCourses,
            TotalEnrollments = totalEnrollments,
            TopCourses = top3Courses,
            UserGrowthPerMonths = userGrowthPerMonths
        };

        return result;
    }
}
