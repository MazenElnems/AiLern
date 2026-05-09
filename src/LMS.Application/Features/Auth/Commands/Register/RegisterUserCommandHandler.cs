using AutoMapper;
using LMS.Application.Common.Results;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.Services;
using LMS.Domain.Entities.Users;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace LMS.Application.Features.Auth.Commands.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result>
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IBackgroundJobService _backgroundJobService;

    public RegisterUserCommandHandler(IMapper mapper, UserManager<ApplicationUser> userManager, IBackgroundJobService backgroundJobService, IEmailSender emailSender)
    {
        _mapper = mapper;
        _userManager = userManager;
        _backgroundJobService = backgroundJobService;
        _emailSender = emailSender;
    }

    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user != null)
            return DomainErrors.User.AlreadyExists;

        if (request.Role == Roles.Admin)
            return DomainErrors.Auth.InvalidRole;

        var newUser = _mapper.Map<ApplicationUser>(request);

        var createResult = await _userManager.CreateAsync(newUser, request.Password);

        if (!createResult.Succeeded)
            return DomainErrors.User.CreationFailed(string.Join(", ", createResult.Errors.Select(e => e.Description)));

        var addToRoleResult = await _userManager.AddToRoleAsync(newUser, request.Role.ToString());

        if(!addToRoleResult.Succeeded)
            return DomainErrors.User.RoleAssignmentFailed(request.Role.ToString());

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        _backgroundJobService.Enqueue(
            () => _emailSender.SendWelcomeEmailAsync(newUser.Email!, newUser.FullName)
        );

        _backgroundJobService.Enqueue(
            () => _emailSender.SendConfirmationEmailAsync(newUser.Email!, newUser.FullName, encodedToken)
        );

        return Result.Success("User Registered Successfully");
    }
}
