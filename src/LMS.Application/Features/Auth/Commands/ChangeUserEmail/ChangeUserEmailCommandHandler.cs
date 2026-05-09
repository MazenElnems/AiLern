using LMS.Application.Common.Results;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.Services;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.ChangeUserEmail;

public class ChangeUserEmailCommandHandler : IRequestHandler<ChangeUserEmailCommand, Result>
{
    private readonly IUserContext _user;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly IEmailSender _emailSender;

    public ChangeUserEmailCommandHandler(IUserContext user, UserManager<ApplicationUser> userManager, IBackgroundJobService backgroundJobService, IEmailSender emailSender)
    {
        _user = user;
        _userManager = userManager;
        _backgroundJobService = backgroundJobService;
        _emailSender = emailSender;
    }

    public async Task<Result> Handle(ChangeUserEmailCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.GetCurrentUser().Id;
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
        {
            return DomainErrors.User.InvalidPassword;
        }
        var token = await _userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);

        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        _backgroundJobService.Enqueue(
            () => _emailSender.SendChangeEmailConfirmationAsync(user.Id, request.NewEmail, user.FullName, encodedToken)
        );
        return Result.Success("Email change confirmation sent successfully");
    }
}
