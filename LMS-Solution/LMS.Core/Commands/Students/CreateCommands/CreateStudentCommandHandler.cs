using AutoMapper;
using LMS.Core.CurrentUser;
using LMS.Core.CustomExceptions;
using LMS.Domin.Entities;
using LMS.Domin.RepositoriesInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Students.CreateCommands;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand>
{
    private readonly IMapper _mapper;
    private readonly IUserContext _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CreateStudentCommandHandler> _logger;
    private readonly IUsersRepository _usersRepository;

    public CreateStudentCommandHandler(IMapper mapper, IUserContext currentUser, UserManager<ApplicationUser> user, ILogger<CreateStudentCommandHandler> logger, IUsersRepository usersRepository)
    {
        _mapper = mapper;
        _currentUser = currentUser;
        _userManager = user;
        _logger = logger;
        _usersRepository = usersRepository;
    }

    public async Task Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var student = _mapper.Map<Student>(request);

            var CurrentUser = _currentUser.GetCurrentUser()
                ?? throw new UnAuthorizedException("User is not authenticated");

            student.CreatedBy = CurrentUser.UserName;
            student.CreatedAt = DateTime.UtcNow;
            
            if(_usersRepository.GetStudentByStudentId(request.StudentId) != null)
                throw new UserCreationException("the student id is already exists");

            var result = await _userManager.CreateAsync(student,request.Password);
            if (!result.Succeeded)
                throw new UserCreationException(message:string.Join(", ",result.Errors.Select(e=>e.Description)));
        }
        catch(UserCreationException ex)
        {
            throw;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,$"An error occurred while Create Student with ID {request.StudentId}");
            throw;
        }
    }
}
