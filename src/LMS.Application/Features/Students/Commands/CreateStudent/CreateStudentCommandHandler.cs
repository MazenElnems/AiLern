using AutoMapper;
using LMS.Application.CurrentUser;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common.Errors;
using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace LMS.Application.Features.Students.Commands.CreateStudent;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<int>>
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

    public async Task<Result<int>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var student = _mapper.Map<Student>(request);

            if(await _unitOfWork.Users.GetStudentByStudentId(request.StudentId) != null)
                return Result<int>.Failure(DomainErrors.User.AlreadyExists);

            var result = await _userManager.CreateAsync(student,request.Password);
            if (!result.Succeeded)
                return Result<int>.Failure(DomainErrors.User.CreationFailed(string.Join(", ",result.Errors.Select(e=>e.Description))));

            return Result<int>.Success(student.Id);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex,$"An error occurred while Create Student with ID {request.StudentId}");
            throw;
        }
    }
}
