using LMS.Application.Common.Results;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.ResendConfirmEmail;

public class ResendEmailConfirmationCommandHandler : IRequestHandler<ResendEmailConfirmationCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IBackgroundJobService _backgroundService;

    public ResendEmailConfirmationCommandHandler(
        IEmailSender emailSender,
        UserManager<ApplicationUser> userManager,
        IBackgroundJobService backgroundService)
    {
        _emailSender = emailSender;
        _userManager = userManager;
        _backgroundService = backgroundService;
    }

    public async Task<Result> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user == null)
            return DomainErrors.User.NotFound(request.Email);

        var tokn = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var endcodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokn));

        _backgroundService.Enqueue(() => _emailSender.SendConfirmationEmailAsync(user.Email!, user.FullName, endcodedToken));

        return Result.Success("sending email confirmation");
    }
}