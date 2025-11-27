using AutoMapper;
using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Admins.CreateAdminCommands;

public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand,int>
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

    public async Task<int> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var admin = _mapper.Map<Admin>(request);

            var result = await _userManager.CreateAsync(admin, request.Password);

            if (!result.Succeeded)
                throw new UserCreationException("an error occures while inserting new admin");

            return admin.Id;
        }
        catch(UnAuthorizedException ex)
        {
            throw;
        }
        catch(UserCreationException ex)
        {
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "an error occures while inserting new admin");
            throw;
        }
    }
}
