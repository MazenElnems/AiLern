using AutoMapper;
using LMS.Core.CurrentUser;
using LMS.Domin.Contracts;
using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Students.CreateCommands;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand,int>
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

    public async Task<int> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var student = _mapper.Map<Student>(request);

            var CurrentUser = _currentUser.GetCurrentUser();

            if(_usersRepository.GetStudentByStudentId(request.StudentId) != null)
                throw new UserCreationException("the student id is already exists");

            var result = await _userManager.CreateAsync(student,request.Password);
            if (!result.Succeeded)
                throw new UserCreationException(message:string.Join(", ",result.Errors.Select(e=>e.Description)));

            return student.Id;
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
