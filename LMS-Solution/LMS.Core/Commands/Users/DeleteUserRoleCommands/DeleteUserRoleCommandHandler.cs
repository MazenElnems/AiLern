using LMS.Core.Constants;
using LMS.Core.CustomExceptions;
using LMS.Domin.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Users.DeleteUserRoleCommands;

public class DeleteUserRoleCommandHandler : IRequestHandler<DeleteUserRoleCommand>
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

    public async Task Handle(DeleteUserRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString())
                ?? throw new ResourceNotFoundException(nameof(ApplicationUser), request.Id.ToString());
            var role = await _roleManager.FindByNameAsync(request.Role)
                ?? throw new ResourceNotFoundException(nameof(IdentityRole<int>), request.Id.ToString());

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Count < 2)
                throw new InvalidOperationException("Cannot remove this role because it’s the user’s only role.");

            if (role.Name.ToLower() == UserRoles.Admin.ToLower())
                throw new InvalidOperationException("Cannot remove the Admin role from a user.");

            var result = await _userManager.RemoveFromRoleAsync(user, request.Role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to remove role: {errors}");
            }
        }
        catch (InvalidOperationException ex)
        {
            throw;
        }
        catch (ResourceNotFoundException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"An error occurred while deleting role \"{request.Role}\" from user with ID {request.Id}");
            throw;
        }
    }
}