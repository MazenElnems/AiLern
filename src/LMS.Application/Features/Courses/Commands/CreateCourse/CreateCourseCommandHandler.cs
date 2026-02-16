using AutoMapper;
using LMS.Application.CurrentUser;
using LMS.Domain.Repositories;
using LMS.Application.Common.Results.Generic;
using MediatR;
using Microsoft.Extensions.Logging;
using LMS.Domain.Entities.Courses;

namespace LMS.Application.Features.Courses.Commands.CreateCourse;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCourseCommandHandler> _logger;
    private readonly IUserContext _userContext;

    public CreateCourseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CreateCourseCommandHandler> logger, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<Result<int>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var currentUser = _userContext.GetCurrentUser();

            var course = _mapper.Map<Course>(request);
            course.InstructorId = currentUser.Id;

            _logger.LogInformation("Creating new course {@Course}", request);
            await _unitOfWork.Courses.InsertAsync(course);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("new Course created successfully with ID: {courseId}", course.Id);
            return Result<int>.Success(course.Id);
        }
        catch(Exception ex)
        {
            _logger.LogError("an error occurred while creating new course");
            throw;
        }
    }
}
