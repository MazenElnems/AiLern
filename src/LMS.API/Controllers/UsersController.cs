using LMS.Application.Commands.Users.DeleteUserRoleCommands;
using LMS.Application.Commands.Users.UpdateCommands;
using LMS.Application.Queries.Users.GetAllByRoleIdQueries;
using LMS.Application.Queries.Users.GetByIdQueries;
using LMS.Domain.DTOs;
using LMS.Domain.DTOs.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<GetUserByIdDto>> GetById(int id)
    {
        var dto = await _mediator.Send(new GetUserByIdQuery(id));
        return dto;
    }
    [HttpPut]
    [Route("{id}/roles")]
    public async Task<IActionResult> AddRole(int id, AddUserToRoleCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("roles/{roleid}")]
    public async Task<ActionResult<PaginationResult<GetUsersByRoleDto>>> GetUsersByRoleId(int roleid, [FromQuery] GetAllByRoleIdQuery query)
    {
        query.RoleId = roleid;
        var dto = await _mediator.Send(query);
        return dto;
    }

    [HttpDelete("{id}/roles")]
    public async Task<IActionResult> DeleteUserRole(int id, DeleteUserRoleCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }
}
