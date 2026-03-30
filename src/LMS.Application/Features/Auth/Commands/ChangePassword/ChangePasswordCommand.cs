using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(
    string CurrentPasswor,
    string NewPassword
) : IRequest<Result>;
