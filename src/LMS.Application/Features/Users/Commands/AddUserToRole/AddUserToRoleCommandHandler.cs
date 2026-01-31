using LMS.Application.Common.Results;
using LMS.Domain.Common.Errors;
using LMS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.Extensions.Logging;
using LMS.Application.Features.Courses.Commands.UpdateCourse;

namespace LMS.Application.Features.Users.Commands.AddUserToRole;

public class AddUserToRoleCommandHandler : IRequestHandler<AddUserToRoleCommand, Result>
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

    public async Task<Result> Handle(AddUserToRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString())
            ;
        if (user == null)
            return Result.Failure(DomainErrors.User.NotFound(request.Id.ToString()));
        var exist = await _roleManager.RoleExistsAsync(request.Role);
        if (!exist)
        {
            return Result.Failure(DomainErrors.Role.NotFound(request.Role));
        }

        var result = await _userManager.AddToRoleAsync(user,request.Role);

        if(!result.Succeeded)
        {
            _logger.LogWarning( "An error occurred while Adding role: {RoleName} to user: {Userid}", request.Role,user.Id);
            var message = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure(DomainErrors.Common.BusinessRule("User.AddRoleFailed", message));
        }
        return Result.Success();
    }
}
