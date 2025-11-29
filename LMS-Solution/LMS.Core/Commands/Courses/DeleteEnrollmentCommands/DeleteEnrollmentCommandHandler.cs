using LMS.Core.Commands.Courses.ApproveEntrollmentsCommands;
using LMS.Domin.Contracts;
using LMS.Domin.Entities;
using LMS.Domin.Enums;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Commands.Courses.DeleteEnrollmentCommands
{
    public class DeleteEnrollmentCommandHandler : IRequestHandler<DeleteEnrollmentCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<DeleteEnrollmentCommandHandler> _logger;

        public DeleteEnrollmentCommandHandler(ICourseRepository course, ILogger<DeleteEnrollmentCommandHandler> logger)
        {
            _courseRepository = course;
            _logger = logger;
        }

        public async Task Handle(DeleteEnrollmentCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var enrollment = await _courseRepository.GetEnrollmentByIdAsync(request.CourseId, request.StudentId)
                    ?? throw new ResourceNotFoundException(nameof(Enrollment), $"{{{request.CourseId}, {request.StudentId}}}");

                _logger.LogInformation("Deleting enrollment with course ID {CourseId} and student ID {StudentId}",
                                            request.CourseId, request.StudentId);

                var effectedRows = await _courseRepository.RemoveEnrollmentAsync(enrollment);

                if (effectedRows < 1)
                    throw new Exception();

                _logger.LogInformation("Enrollment with course ID {CourseId} and student ID {StudentId} Deleted successfully",
                                            request.CourseId, request.StudentId);
            }
            catch (CourseUpdateException)
            {
                throw;
            }
            catch (ResourceNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Deleting enrollment with course ID {CourseId} and student ID {StudentId}",
                                            request.CourseId, request.StudentId);
            }
        }
    }
}
