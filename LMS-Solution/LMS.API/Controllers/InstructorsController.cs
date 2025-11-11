using LMS.Core.Commands.Instructors.CreateInstructorsCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InstructorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InstructorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Create(CreateInstructorCommand command)
    {
        await _mediator.Send(command);
        return Created();
    }
}
