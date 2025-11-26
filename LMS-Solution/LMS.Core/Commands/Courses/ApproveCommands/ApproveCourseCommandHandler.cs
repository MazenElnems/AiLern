using LMS.Core.CurrentUser;
using LMS.Core.CustomExceptions;
using LMS.Domin.Entities;
using LMS.Domin.Enums;
using LMS.Domin.RepositoriesInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Commands.Courses.ApproveCommands
{
    public class ApproveCourseCommandHandler : IRequestHandler<ApproveCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<ApproveCourseCommandHandler> _logger;
        private readonly IUserContext _userContext;


        public ApproveCourseCommandHandler(ICourseRepository courseRepository, ILogger<ApproveCourseCommandHandler> logger, IUserContext userContext)
        {
            _courseRepository = courseRepository;
            _logger = logger;
            _userContext = userContext;
        }

        public async Task Handle(ApproveCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var currentUser = _userContext.GetCurrentUser();
                var course = await _courseRepository.GetByIdAsync(request.Id)
                     ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString());
                if(course.CourseStatus != CourseStatus.Pending )
                {
                    throw new CourseUpdateException();
                }
                _logger.LogInformation("Approving course with ID {CourseId}", request.Id);
                course.CourseStatus = CourseStatus.Approved;
                //course.Approvedby = currentUser!.Id;
                course.Approvedby = 3;
                course.ApprovedDate = DateTime.UtcNow;

                _logger.LogInformation("Course with ID {CourseId} Approved successfully", request.Id);
                await _courseRepository.CommitAsync();
            }
            catch (ResourceNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Approving course with ID {CourseId}", request.Id);
                throw;
            }
        }
    }
}
