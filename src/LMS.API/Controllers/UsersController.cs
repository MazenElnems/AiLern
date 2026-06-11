using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Features.Instructors.Queries.GetMyCourses;
using LMS.Application.Features.Instructors.Queries.GetTopThreeCourseProgress;
using LMS.Application.Features.Report.Commands.ApproveReport;
using LMS.Application.Features.Report.Queries.GetStatistics;
using LMS.Application.Features.Students.Queries.GetMyCourses;
using LMS.Application.Features.Students.Queries.GetStudentProfileInCourse;
using LMS.Application.Features.Users.Commands.AddUserToRole;
using LMS.Application.Features.Users.Commands.DeleteUserRole;
using LMS.Application.Features.Users.Queries.GetAllByRoleId;
using LMS.Application.Features.Users.Queries.GetMe;
using LMS.Application.Features.Users.Queries.GetUserById;
using LMS.Domain.Constants;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("User and role management endpoints.")]
public class UsersController : ApiBaseController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get user by ID", Description = "Retrieves user details by ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetById(int id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        return HandleResponse(this, result);
    }
    [HttpPut("{id}/roles")]
    [SwaggerOperation(Summary = "Add user role", Description = "Adds a role to a user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Role added successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User or role not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> AddRole(int id, AddUserToRoleCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet("roles")]
    [SwaggerOperation(Summary = "Get users by role", Description = "Retrieves users assigned to a role.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Users retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid query parameters.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Role not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetUsersByRole(Roles role, int pageNo = 1, int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllByRoleQuery(role, pageNo, pageSize));
        return HandleResponse(this, result);
    }
    [HttpPut("admin/content-reports")]
    [Authorize(Roles = UserRoles.Admin)]
    [SwaggerOperation(Summary = "Approve content report", Description = "Approves a content report.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Report approved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Report not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> ApproveReport(Guid reportid)
    {
        var result = await _mediator.Send(new ApproveMaterialReportCommand { ReportId = reportid });
        return HandleResponse(this, result);
    }

    [HttpGet("admin/content-reports")]
    [Authorize(Roles = UserRoles.Admin)]
    [SwaggerOperation(Summary = "Get reports statistics", Description = "Retrieves statistics for content reports.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Statistics retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Report not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetStatistics()
    {
        var result = await _mediator.Send(new GetReportsStatisticsQuery());
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}/roles")]
    [SwaggerOperation(Summary = "Delete user role", Description = "Removes a role from a user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Role removed successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User or role not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> DeleteUserRole(int id, DeleteUserRoleCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet("students/my-courses") ]
    [Authorize(Roles = UserRoles.Student)]
    [SwaggerOperation(Summary = "Get my courses", Description = "Retrieves the current student's courses.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Courses retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid query parameters.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetCourses(int pageNo = 1, int pageSize = 10)
    {
        var result = await _mediator.Send(new GetStudentCoursesQuery(pageNo, pageSize));
        return HandleResponse(this, result);
    }

    [HttpGet("instructor/my-courses") ]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Get my courses", Description = "Retrieves the current instructor's courses.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Courses retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid query parameters.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetInstructorCourses()
    {
        var result = await _mediator.Send(new GetInstructorCoursesQuery());
        return HandleResponse(this, result);
    }

    [HttpGet("instructor/my-courses-progress") ]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Get my courses progress", Description = "Retrieves the current instructor's courses progress.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Courses progress retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid query parameters.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetInstructorCoursesProgress()
    {
        var result = await _mediator.Send(new GetTopThreeCourseProgressQuery());
        return HandleResponse(this, result);
    }

    [HttpGet("student-profile") ]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Get student profile in course", Description = "Retrieves the profile of a student in a specific course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Student profile retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid query parameters.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetStudentProfileInCourse(int CourseId , int StudentId)
    {
        var result = await _mediator.Send(new GetStudentProfileInCourseQuery { CourseId = CourseId, StudentId = StudentId });
        return HandleResponse(this, result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> GetMe()
    {
        var result = await _mediator.Send(new GetMeQuery());
        return HandleResponse(this, result);
    }

}
