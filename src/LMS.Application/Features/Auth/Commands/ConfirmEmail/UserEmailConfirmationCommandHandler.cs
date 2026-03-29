using LMS.Application.Common.Results;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace LMS.Application.Commands.Auth.EmailConfirmationCommands;

public class UserEmailConfirmationCommandHandler : IRequestHandler<UserEmailConfirmationCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserEmailConfirmationCommandHandler> _logger;

    public UserEmailConfirmationCommandHandler(ILogger<UserEmailConfirmationCommandHandler> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<Result> Handle(UserEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
            
        if (user == null)
            return DomainErrors.User.NotFound(request.Email);

        if(user.EmailConfirmed)
        {
            _logger.LogInformation("Email already confirmed for user with email {Email}", request.Email);
            return Result.Success("Email already confirmed.");
        }

        _logger.LogInformation("Confirming email for user with email {Email}", request.Email);

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Email confirmation failed for user with email {Email}. Errors: {Errors}", request.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
            return DomainErrors.Auth.EmailConfirmationFailed;
        }

        return Result.Success("Email confirmed successfully.");
    }
}
