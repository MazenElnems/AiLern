using LMS.Core.Commands.Courses.UpdateCommands;
using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Users.UpdateCommands;

public class AddUserToRoleCommandHandler : IRequestHandler<AddUserToRoleCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ILogger<UpdateCourseDetailsCommandHandler> _logger;

    public AddUserToRoleCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, ILogger<UpdateCourseDetailsCommandHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task Handle(AddUserToRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString())
            ?? throw new ResourceNotFoundException(nameof(ApplicationUser), request.Id.ToString());
        var exist = await _roleManager.RoleExistsAsync(request.Role);
        if (!exist)
        {
            throw new ResourceNotFoundException(nameof(IdentityRole<int>),request.Role);
        }

        var result = await _userManager.AddToRoleAsync(user,request.Role);

        if(!result.Succeeded)
        {
            _logger.LogWarning( "An error occurred while Adding role: {RoleName} to user: {Userid}", request.Role,user.Id);
        }
        
    }
}
