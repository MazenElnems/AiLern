using LMS.Application.Commands.Auth.EmailConfirmationCommands;
using LMS.Application.Commands.Auth.PasswordReSetCommands;
using LMS.Application.Commands.Auth.ResendEmailConfirmationCommands;
using LMS.Application.Commands.Auth.SendPasswordResetEmailCommands;
using LMS.Application.Commands.Auth.UserLoginCommands;
using LMS.Application.Commands.Auth.UserRefreshTokenCommands;
using LMS.Application.Commands.Auth.UserRevokeRefreshTokenCommands;
using LMS.Application.DTOs.Auth.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<GetTokenResponseDto>> RefreshToken(GetRefreshTokenCommand command)
    {
        var dto = await _mediator.Send(command);
        return dto;
    }

    [HttpPost("login")]
    public async Task<ActionResult<GetTokenResponseDto>> Login(UserLoginByEmailAndPasswordCommand command)
    {
        var dto = await _mediator.Send(command);
        return dto;
    }

    [HttpPut("revoke-token")]
    public async Task<IActionResult> RevokeRefreshToken(RevokeRefreshTokenCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] UserEmailConfirmationCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail(ResendEmailConfirmationCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("send-password-reset-email")]
    public async Task<IActionResult> SendPasswordResetEmail(SendPasswordResetEmailCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(UserPasswordResetCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}
