using LMS.Core.Commands.Admins.CreateAdminCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Create(CreateAdminCommand command)
    {
        await _mediator.Send(command);
        return Created();
    }
}
