using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Features.Notifications.Commands.MarkAllAsRead;
using LMS.Application.Features.Notifications.Queries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ApiBaseController
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("Notifications")]
    [Authorize]
    [SwaggerOperation(Summary = "Get notifications", Description = "Retrieves notifications for the current user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Notifications retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetNotifications(int pageNo = 1, int pageSize = 10)
    {
        var result = await _mediator.Send(new GetNotificationsQuery { PageNumber = pageNo, PageSize = pageSize });
        return HandleResponse(this, result);
    }

    [HttpPut("MarkAllAsRead")]
    [Authorize]
    [SwaggerOperation(Summary = "Mark all notifications as read", Description = "Marks all notifications for the current user as read.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Notifications marked as read successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> MarkAllAsRead()
    {
        var result = await _mediator.Send(new MarkAllAsReadCommand());
        return HandleResponse(this, result);
    }
}
