using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword
) : IRequest<Result>;
