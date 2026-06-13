using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(int userId):IRequest<Result>;
