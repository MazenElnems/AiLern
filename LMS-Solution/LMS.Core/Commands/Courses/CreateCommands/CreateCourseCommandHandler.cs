using AutoMapper;
using LMS.Core.Domain.Entities;
using LMS.Core.Domain.RepositoriesInterfaces;
using LMS.Core.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Courses.CreateCommands;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, int>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCourseCommandHandler> _logger;
    private readonly IUserContext _userContext;

    public CreateCourseCommandHandler(ICourseRepository courseRepository, IMapper mapper, ILogger<CreateCourseCommandHandler> logger, IUserContext userContext)
    {
        _courseRepository = courseRepository;
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
            course.InstructorId = currentUser!.Id;

            _logger.LogInformation("Creating new course {@Course}", request);
            int id = await _courseRepository.AddAsync(course);
            _logger.LogInformation("new Course created successfully with ID: {courseId}", course.Id);
            return id;
        }
        catch(Exception ex)
        {
            _logger.LogError("an error occurred while creating new course");
            throw;
        }
    }
}
