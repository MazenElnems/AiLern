using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Features.Instructors.Queries.GetMyDashboard;
using LMS.Application.Features.Instructors.Queries.GetUpcomingEvents;
using LMS.Application.Features.Dashboards.Queries.GetQuizDashboard;
using LMS.Domain.Constants;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.API.Controllers;

namespace LMS.API.Controllers
{
[Route("api/[controller]")]
[ApiController]
public class DashboardController : ApiBaseController
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]

    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Get instructor dashboard ", Description = "Retrieves instructor dashboard.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Instructor dashboard retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetInstructorDashboard()
        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<ApiResponse>> GetQuizDashboard([FromRoute]GetQuizDashboardQuery query)
    {
        var result = await _mediator.Send(new GetInstructorDashboardQuery());
            var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }
    [HttpGet("UpcomingEvents")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Get upcoming Events ", Description = "Retrieves upcoming Events.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Upcoming Events retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetUpcomingEvents(EventType eventType, int pageNo = 1, int pageSize = 10)
    {
        var result = await _mediator.Send(new GetUpcomingEventsQuery(eventType,pageNo, pageSize));
        return HandleResponse(this, result);
    }
}
