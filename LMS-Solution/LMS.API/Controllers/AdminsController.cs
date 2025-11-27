using LMS.Core.Commands.Admins.CreateAdminCommands;
using LMS.Domin.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMailSender emailSender;

    public AdminsController(IMediator mediator, IMailSender emailSender)
    {
        _mediator = mediator;
        this.emailSender = emailSender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Create(CreateAdminCommand command)
    {
        await _mediator.Send(command);
        return Created();
    }
}
