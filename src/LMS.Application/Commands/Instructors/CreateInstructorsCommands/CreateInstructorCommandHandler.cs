using AutoMapper;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Commands.Instructors.CreateInstructorsCommands;

public class CreateInstructorCommandHandler : IRequestHandler<CreateInstructorCommand,int>
{
    private readonly IMapper _mapper;
    private readonly IUserContext _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CreateInstructorCommandHandler> _logger;

    public CreateInstructorCommandHandler(ILogger<CreateInstructorCommandHandler> logger, UserManager<ApplicationUser> user, IUserContext currentUser, IMapper mapper)
    {
        _logger = logger;
        _userManager = user;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateInstructorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var instructor = _mapper.Map<Instructor>(request);

            var currentUser = _currentUser.GetCurrentUser();

            var result = await _userManager.CreateAsync(instructor, request.Password);

            if (!result.Succeeded)
                throw new UserCreationException();

            return instructor.Id;
        }
        catch(UserCreationException ex)
        {
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,$"An error occurred while Create Instructor");
            throw;
        }
    }
}
