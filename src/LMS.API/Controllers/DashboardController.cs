using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Features.Admin.Queries.GetAdminDashboard;
using LMS.Application.Features.Dashboards.Queries.CourseDashboard;
using LMS.Application.Features.Dashboards.Queries.GetQuizDashboard;
using LMS.Application.Features.Instructors.Queries.GetMyDashboard;
using LMS.Application.Features.Instructors.Queries.GetUpcomingEvents;
using LMS.Application.Features.Quizzes.Queries.GetAiGradingDashboard;
using LMS.Application.Features.Quizzes.Queries.GetAiQuestionGenerationDashboard;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ApiBaseController
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("instructor")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Get instructor dashboard ", Description = "Retrieves instructor dashboard.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Instructor dashboard retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetInstructorDashboard()
    {
        var result = await _mediator.Send(new GetInstructorDashboardQuery());
        return HandleResponse(this, result);
    }

    [HttpGet("admin")]
    [Authorize(Roles = UserRoles.Admin)]
    [SwaggerOperation(Summary = "Get admin dashboard ", Description = "Retrieves admin dashboard.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Admin dashboard retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetAdminDashboard()
    {
        var result = await _mediator.Send(new GetAdminDashboardQuery());
        return HandleResponse(this, result);
    }

    [HttpGet("quiz/{quizId}")]
    public async Task<ActionResult<ApiResponse>> GetQuizDashboard(Guid quizId)
    {
        var result = await _mediator.Send(new GetQuizDashboardQuery(quizId));
        return HandleResponse(this, result);
    }

    [HttpGet("UpcomingEvents")]
    [Authorize(Roles = UserRoles.Instructor + "," + UserRoles.Student)]
    [SwaggerOperation(Summary = "Get upcoming Events ", Description = "Retrieves upcoming Events.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Upcoming Events retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetUpcomingEvents(EventType? eventType = null, int pageNo = 1, int pageSize = 10)
    {
        var result = await _mediator.Send(new GetUpcomingEventsQuery(eventType, pageNo, pageSize));
        return HandleResponse(this, result);
    }

    [HttpGet("course/{courseId}")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> GetCourseDashboard(int courseId)
    {
        var result = await _mediator.Send(new GetCourseDashboardQuery(courseId));
        return HandleResponse(this, result);
    }

    [HttpGet("admin/ai_question_generation_dashboard")]
    [Authorize(Roles = UserRoles.Admin)]
    [SwaggerOperation(Summary = "Get ai question generation dashboard ", Description = "Retrieves ai question generation dashboard.")]
    [SwaggerResponse(StatusCodes.Status200OK, "ai question generation dashboard retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetAiQuestionGenerationDashboard()
    {
        var result = await _mediator.Send(new GetAiQuestionGenerationDashboardQuery());
        return HandleResponse(this, result);
    }

    [HttpGet("admin/ai_grading_dashboard")]
    [Authorize(Roles = UserRoles.Admin)]
    [SwaggerOperation(Summary = "Get ai grading dashboard ", Description = "Retrieves ai grading dashboard.")]
    [SwaggerResponse(StatusCodes.Status200OK, "ai grading dashboard retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetAiGradingDashboard()
    {
        var result = await _mediator.Send(new GetAiGradingDashboardQuery());
        return HandleResponse(this, result);
    }
}

