using LMS.Application.Commands.Auth.EmailConfirmationCommands;
using LMS.API.Common.Responses;
using LMS.API.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LMS.Application.Features.Auth.Commands.Login;
using LMS.Application.Features.Auth.Commands.ResendConfirmEmail;
using LMS.Application.Features.Auth.Commands.RefreshTokens;
using LMS.Application.Features.Auth.Commands.RevokeToken;
using LMS.Application.Features.Auth.Commands.ResetPassword;
using LMS.Application.Features.Auth.Commands.PasswordResetEmail;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ApiBaseController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse>> RefreshToken(GetRefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse>> Login(UserLoginByEmailAndPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPut("revoke-token")]
    public async Task<ActionResult<ApiResponse>> RevokeRefreshToken(RevokeRefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet("confirm-email")]
    public async Task<ActionResult<ApiResponse>> ConfirmEmail([FromQuery] UserEmailConfirmationCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("resend-confirmation-email")]
    public async Task<ActionResult<ApiResponse>> ResendConfirmationEmail(ResendEmailConfirmationCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("send-password-reset-email")]
    public async Task<ActionResult<ApiResponse>> SendPasswordResetEmail(SendPasswordResetEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<ApiResponse>> ChangePassword(UserPasswordResetCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }
}
