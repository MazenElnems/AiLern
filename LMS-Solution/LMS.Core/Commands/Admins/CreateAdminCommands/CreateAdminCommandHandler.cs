using AutoMapper;
using LMS.Core.CurrentUser;
using LMS.Core.CustomExceptions;
using LMS.Domin.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Admins.CreateAdminCommands;

public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand,int>
{
    private readonly IMapper _mapper;
    private readonly IUserContext _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CreateAdminCommandHandler> _logger;

    public CreateAdminCommandHandler(ILogger<CreateAdminCommandHandler> logger, UserManager<ApplicationUser> user, IUserContext currentUser, IMapper mapper)
    {
        _logger = logger;
        _userManager = user;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var admin = _mapper.Map<Admin>(request);

            var currentUser = _currentUser.GetCurrentUser()
                ?? throw new UnAuthorizedException("User isn't authenticated");

            admin.CreatedBy = currentUser.UserName;
            admin.CreatedAt = DateTime.UtcNow;

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
