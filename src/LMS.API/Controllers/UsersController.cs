using LMS.API.Controllers.Common;
using LMS.API.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LMS.Application.Features.Users.Commands.AddUserToRole;
using LMS.Application.Features.Users.Commands.DeleteUserRole;
using LMS.Application.Features.Users.Queries.GetAllByRoleId;
using LMS.Application.Features.Users.Queries.GetUserById;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ApiBaseController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ApiResponse>> GetById(int id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        return HandleResponse(this, result);
    }
    [HttpPut]
    [Route("{id}/roles")]
    public async Task<ActionResult<ApiResponse>> AddRole(int id, AddUserToRoleCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet("roles/{roleid}")]
    public async Task<ActionResult<ApiResponse>> GetUsersByRoleId(int roleid, [FromQuery] GetAllByRoleIdQuery query)
    {
        query.RoleId = roleid;
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}/roles")]
    public async Task<ActionResult<ApiResponse>> DeleteUserRole(int id, DeleteUserRoleCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }
}
