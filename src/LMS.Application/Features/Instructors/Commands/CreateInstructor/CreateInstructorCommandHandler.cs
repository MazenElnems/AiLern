using AutoMapper;
using LMS.Application.CurrentUser;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common.Errors;
using LMS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace LMS.Application.Features.Instructors.Commands.CreateInstructor;

public class CreateInstructorCommandHandler : IRequestHandler<CreateInstructorCommand, Result<int>>
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

    public async Task<Result<int>> Handle(CreateInstructorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var instructor = _mapper.Map<Instructor>(request);

            var result = await _userManager.CreateAsync(instructor, request.Password);

            if (!result.Succeeded)
            {
                var message = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<int>.Failure(DomainErrors.User.CreationFailed(message));
            }

            return Result<int>.Success(instructor.Id);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,$"An error occurred while Create Instructor");
            throw;
        }
    }
}
