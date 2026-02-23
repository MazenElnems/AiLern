using LMS.Application.ConfigurationOptions;
using LMS.Application.Common.Results;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using Microsoft.Extensions.Configuration;
using LMS.Domain.Interfaces;

namespace LMS.Application.Features.Auth.Commands.PasswordResetEmail;

public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IBackgroundService _backgroundService;

    public ForgetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IBackgroundService backgroundService)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _backgroundService = backgroundService;
    }

    public async Task<Result> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if(user == null)
            return DomainErrors.User.NotFound(request.Email);  

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        _backgroundService.Enqueue(() => _emailSender.SendForgetPasswordEmailAsync(user.Email!, user.FullName, token));

        return Result.Success("sending forget password email");
    }
}
