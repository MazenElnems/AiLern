using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Instructors.Shared.DTO;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Instructors.Queries.GetMyDashboard;

public class GetInstructorDashboardQueryHandler : IRequestHandler<GetInstructorDashboardQuery, Result<InstructorDashboardDto>>
{
    private readonly IUserContext _user;
    private readonly IUnitOfWork _unitOfWork ;
    private readonly ILogger<GetInstructorDashboardQueryHandler> _logger;

    public GetInstructorDashboardQueryHandler(IUserContext user, IUnitOfWork unitOfWork, ILogger<GetInstructorDashboardQueryHandler> logger)
    {
        _user = user;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<InstructorDashboardDto>> Handle(GetInstructorDashboardQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _user.GetCurrentUser().Id;

            var statistic = await _unitOfWork.Courses.Query
                .AsNoTracking()
                .Where(c => c.InstructorId == userId)
                .GroupBy(c => 1)
                .Select(g => new InstructorDashboardDto
                {
                    TotalCourses = g.Count(),

                    TotalStudents = g
                        .SelectMany(c => c.Enrollments)
                        .Select(e => e.StudentId)
                        .Distinct()
                        .Count(),

                    TotalQuizzes = g
                        .SelectMany(c => c.Quizzes)
                        .Count(),

                    TotalAssignments = g
                        .SelectMany(c => c.Assignments)
                        .Count()
                })
                .FirstOrDefaultAsync();




            return Result<InstructorDashboardDto>.Success(statistic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching instructor dashboard for user {UserId}", _user.GetCurrentUser().Id);
            throw;
        }
    }
}
