using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Commands.Auth.EmailConfirmationCommands;
using LMS.Application.Features.Auth.Commands.ChangePassword;
using LMS.Application.Features.Auth.Commands.ChangePhoto;
using LMS.Application.Features.Auth.Commands.ChangeUserEmail;
using LMS.Application.Features.Auth.Commands.ConfirmChangeEmail;
using LMS.Application.Features.Auth.Commands.DeletePhoto;
using LMS.Application.Features.Auth.Commands.Login;
using LMS.Application.Features.Auth.Commands.PasswordResetEmail;
using LMS.Application.Features.Auth.Commands.RefreshTokens;
using LMS.Application.Features.Auth.Commands.Register;
using LMS.Application.Features.Auth.Commands.ResendConfirmEmail;
using LMS.Application.Features.Auth.Commands.ResetPassword;
using LMS.Application.Features.Auth.Commands.RevokeToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("Authentication and user account operations.")]
public class AuthController : ApiBaseController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(Summary = "Refresh access token", Description = "Exchanges a refresh token for a new access token.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Token refreshed successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Refresh token not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> RefreshToken(GetRefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login", Description = "Authenticates a user by email and password.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Login successful.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid credentials.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Login(UserLoginByEmailAndPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register user", Description = "register a new user account.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User registered successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPut("revoke-token")]
    [Authorize]
    [SwaggerOperation(Summary = "Revoke refresh token", Description = "Revokes a refresh token so it can no longer be used.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Token revoked successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Refresh token not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> RevokeRefreshToken(RevokeRefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet("confirm-email")]
    [SwaggerOperation(Summary = "Confirm email", Description = "Confirms a user's email using the confirmation token.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Email confirmed successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid token or request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> ConfirmEmail([FromQuery] UserEmailConfirmationCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("resend-confirmation-email")]
    [SwaggerOperation(Summary = "Resend confirmation email", Description = "Sends a new email confirmation link.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Confirmation email sent.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> ResendConfirmationEmail(ResendEmailConfirmationCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("forget-password")]
    [SwaggerOperation(Summary = "Send password reset email", Description = "Sends a password reset email to the user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Password reset email sent.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> SendPasswordResetEmail(ForgetPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }
    [HttpPut("change-email")]
    [Authorize]
    [SwaggerOperation(Summary = "Change user email", Description = "Changes the email of a user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Email change confirmation sent successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> ChangeEmail(ChangeUserEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet("confirm-change-email")]
    [SwaggerOperation(Summary = "Confirm change user email", Description = "Confirms the email change of a user.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Email change confirmed successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> ConfirmChangeEmail([FromQuery]ConfirmChangeUserEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("reset-password")]
    [SwaggerOperation(Summary = "Reset password", Description = "Resets a user's password using a reset token.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Password Reseted successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or token.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> ResetPassword(UserPasswordResetCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPost("change-password")]
    [Authorize]
    [SwaggerOperation(Summary = "Change password", Description = "Change password for logged-in users.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Password changed successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Email not found", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> ChangePassword(ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }
    [HttpPut("change-photo")]
    [Authorize]
    [SwaggerOperation(Summary = "Change photo", Description = "Change photo for logged-in users.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Photo changed successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Email not found", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> ChangePhoto([FromBody]ChangeUserPhotoCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpDelete("delete-photo")]
    [Authorize]
    [SwaggerOperation(Summary = "Delete photo", Description = "Delete photo for logged-in users.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Photo deleted     successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Email not found", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> DeletePhoto(DeleteUserPhotoCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }
}
