using LMS.API.Common.Responses;
using LMS.API.Controllers.Common;
using LMS.Application.Features.Admins.Commands.CreateAdmin;
using LMS.Application.Features.Admins.Commands.CreateAdmin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminsController : ApiBaseController
{
    private readonly IMediator _mediator;

    public AdminsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse>> Create(CreateAdminCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }
}
