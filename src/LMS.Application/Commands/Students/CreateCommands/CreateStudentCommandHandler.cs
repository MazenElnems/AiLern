using AutoMapper;
using LMS.Application.CurrentUser;
using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Commands.Students.CreateCommands;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand,int>
{
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CreateStudentCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStudentCommandHandler(IMapper mapper, UserManager<ApplicationUser> user, ILogger<CreateStudentCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _userManager = user;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var student = _mapper.Map<Student>(request);

            if(await _unitOfWork.Users.GetStudentByStudentId(request.StudentId) != null)
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
