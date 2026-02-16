using LMS.Application.Common.Results;
using LMS.Domain.Common.Errors;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Users.Commands.DeleteUserRole;

public class DeleteUserRoleCommandHandler : IRequestHandler<DeleteUserRoleCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ILogger<DeleteUserRoleCommandHandler> _logger;

    public DeleteUserRoleCommandHandler(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager, ILogger<DeleteUserRoleCommandHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteUserRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                return Result.Failure(DomainErrors.User.NotFound(request.Id.ToString()));

            var role = await _roleManager.FindByNameAsync(request.Role);
            if (role == null)
                return Result.Failure(DomainErrors.Role.NotFound(request.Role));

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Count < 2)
                return Result.Failure(DomainErrors.Role.RemoveOnlyRole);

            if (role.Name.ToLower() == UserRoles.Admin.ToLower())
                return Result.Failure(DomainErrors.Role.RemoveAdminRole);

            var result = await _userManager.RemoveFromRoleAsync(user, request.Role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure(DomainErrors.Role.RemoveFailed(errors));
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"An error occurred while deleting role \"{request.Role}\" from user with ID {request.Id}");
            throw;
        }
    }
}