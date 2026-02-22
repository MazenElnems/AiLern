using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Entities.Users;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Admins.Commands.CreateAdmin;

public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, Result<int>>
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CreateAdminCommandHandler> _logger;

    public CreateAdminCommandHandler(ILogger<CreateAdminCommandHandler> logger, UserManager<ApplicationUser> user, IMapper mapper)
    {
        _logger = logger;
        _userManager = user;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var admin = _mapper.Map<Admin>(request);

            var result = await _userManager.CreateAsync(admin, request.Password);

            if (!result.Succeeded)
            {
                var message = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<int>.Failure(DomainErrors.User.CreationFailed(message));
            }

            return Result<int>.Success(admin.Id);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "an error occures while inserting new admin");
            throw;
        }
    }
}
