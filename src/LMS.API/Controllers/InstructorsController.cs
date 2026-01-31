using LMS.API.Common.Responses;
using LMS.API.Controllers.Common;
using LMS.Application.Features.Instructors.Commands.CreateInstructor;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InstructorsController : ApiBaseController
{
    private readonly IMediator _mediator;

    public InstructorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse>> Create(CreateInstructorCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }
}
