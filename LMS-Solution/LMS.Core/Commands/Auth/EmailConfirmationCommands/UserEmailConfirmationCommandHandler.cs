using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Auth.EmailConfirmationCommands;

public class UserEmailConfirmationCommandHandler : IRequestHandler<UserEmailConfirmationCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserEmailConfirmationCommandHandler> _logger;

    public UserEmailConfirmationCommandHandler(ILogger<UserEmailConfirmationCommandHandler> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task Handle(UserEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
            ?? throw new ResourceNotFoundException("User", request.Email);

        if (user.EmailConfirmed)
            return;

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded)
            throw new Exception();
    }
}
