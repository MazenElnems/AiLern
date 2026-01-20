using AutoMapper;
using LMS.Core.CurrentUser;
using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Courses.CreateCommands;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, int>
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

    public async Task<int> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
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
            return course.Id;
        }
        catch(Exception ex)
        {
            _logger.LogError("an error occurred while creating new course");
            throw;
        }
    }
}
