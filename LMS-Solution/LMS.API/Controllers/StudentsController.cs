using LMS.Core.Commands.Students.CreateCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    //[Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create(CreateStudentCommand command)
    {
        await _mediator.Send(command);
        return Created();
    }
}
