using LMS.Core.Commands.Users.DeleteUserRoleCommands;
using LMS.Core.Commands.Users.UpdateCommands;
using LMS.Core.DTOs.Users;
using LMS.Core.Queries.Users.GetAllByRoleIdQueries;
using LMS.Core.Queries.Users.GetByIdQueries;
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
    public async Task<ActionResult<IEnumerable<GetUsersByRoleDto>>> GetUsersByRoleId(int roleid, [FromQuery] GetAllByRoleIdQuery query)
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
