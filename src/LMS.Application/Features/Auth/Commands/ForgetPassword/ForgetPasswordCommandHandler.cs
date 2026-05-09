using LMS.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.Services;

namespace LMS.Application.Features.Auth.Commands.PasswordResetEmail;

public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IBackgroundJobService _backgroundService;

    public ForgetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IBackgroundJobService backgroundService)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _backgroundService = backgroundService;
    }

    public async Task<Result> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if(user == null)
            return DomainErrors.User.EmailNotFound;  

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        _backgroundService.Enqueue(() => _emailSender.SendForgetPasswordEmailAsync(user.Email!, user.FullName, encodedToken));

        return Result.Success("sending forget password email");
    }
}
