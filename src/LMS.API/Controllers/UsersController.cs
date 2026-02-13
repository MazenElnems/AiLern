using LMS.API.Common.Responses;
using LMS.API.Controllers.Common;
using LMS.Application.Features.Students.Queries.GetMyCourses;
using LMS.Application.Features.Users.Commands.AddUserToRole;
using LMS.Application.Features.Users.Commands.DeleteUserRole;
using LMS.Application.Features.Users.Queries.GetAllByRoleId;
using LMS.Application.Features.Users.Queries.GetUserById;
using MediatR;
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

    [HttpGet]
    [Route("{id}")]
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
    [HttpPut]
    [Route("{id}/roles")]
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

    [HttpGet("roles/{roleid}")]
    [SwaggerOperation(Summary = "Get users by role", Description = "Retrieves users assigned to a role.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Users retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid query parameters.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Role not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetUsersByRoleId(int roleid, [FromQuery] GetAllByRoleIdQuery query)
    {
        query.RoleId = roleid;
        var result = await _mediator.Send(query);
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
    [SwaggerOperation(Summary = "Get my courses", Description = "Retrieves the current student's courses.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Courses retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid query parameters.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetCourses([FromQuery] GetStudentCoursesQuery query)
    {
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }
}
