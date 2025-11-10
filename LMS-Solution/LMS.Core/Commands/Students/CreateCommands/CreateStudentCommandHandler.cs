using AutoMapper;
using LMS.Core.CurrentUser;
using LMS.Core.CustomExceptions;
using LMS.Domin.Entities;
using LMS.Domin.RepositoriesInterfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Commands.Students.CreateCommands
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand>
    {
        private readonly IMapper _mapper;
        private readonly IUserContext _currentUser;
        private readonly UserManager<ApplicationUser> _user;
        private readonly ILogger<CreateStudentCommandHandler> _logger;
        private readonly IUsersRepository _usersRepository;

        public CreateStudentCommandHandler(IMapper mapper, IUserContext currentUser, UserManager<ApplicationUser> user, ILogger<CreateStudentCommandHandler> logger, IUsersRepository usersRepository)
        {
            _mapper = mapper;
            _currentUser = currentUser;
            _user = user;
            _logger = logger;
            _usersRepository = usersRepository;
        }

        public async Task Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var student = _mapper.Map<Student>(request);

                var CurrentUser = _currentUser.GetCurrentUser();
                student.CreatedBy = "System";
                student.CreatedAt = DateTime.UtcNow;
                if(_usersRepository.GetStudentByStudentId(request.StudentId) != null)
                {
                    throw new DuplicatedUserException("the student id is already exists");
                }
                var result = await _user.CreateAsync(student,request.Password);
                if (!result.Succeeded)
                {
                
                    throw new DuplicatedUserException(message:string.Join(", ",result.Errors.Select(e=>e.Description)));
                
                }
            }
            catch(DuplicatedUserException ex)
            {
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(
                $"An error occurred while Create Student with ID {request.StudentId}");
                throw;
            }
        }
    }
}
