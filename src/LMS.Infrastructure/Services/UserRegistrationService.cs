using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Jobs;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace LMS.Infrastructure.Services;

public class UserRegistrationService : IUserRegistrationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<UserRegistrationService> _logger;

    public UserRegistrationService(
        UserManager<ApplicationUser> userManager,
        IBackgroundJobService backgroundJobService,
        IEmailSender emailSender,
        ILogger<UserRegistrationService> logger)
    {
        _userManager = userManager;
        _backgroundJobService = backgroundJobService;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<Result<int>> RegisterUserAsync(ApplicationUser user, string password, string role)
    {
        var existingUser = await _userManager.FindByEmailAsync(user.Email!);
        if (existingUser != null)
            return Result<int>.Failure(DomainErrors.User.AlreadyExists);

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var message = string.Join(", ", createResult.Errors.Select(e => e.Description));
            _logger.LogWarning("User registration failed: {Errors}", message);
            return Result<int>.Failure(DomainErrors.User.CreationFailed(message));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            var message = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            _logger.LogWarning("Role assignment failed: {Errors}", message);
            return Result<int>.Failure(DomainErrors.User.RoleAssignmentFailed(role));
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        _backgroundJobService.Enqueue(
            () => _emailSender.SendWelcomeEmailAsync(user.Email!, user.FullName));

        _backgroundJobService.Enqueue(
            () => _emailSender.SendConfirmationEmailAsync(user.Email!, user.FullName, encodedToken));

        return Result<int>.Success(user.Id);
    }
}
