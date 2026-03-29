using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.PasswordResetEmail;

public record ForgetPasswordCommand(string Email) : IRequest<Result>
{ }
