using LMS.Domin.Contracts;
using LMS.Domin.Entities;
using LMS.Domin.Enums;
using LMS.Domin.Exceptions;
using MediatR;

namespace LMS.Core.Queries.Courses.GetStudentsByCourseId
{

    public class GetStudentsByCourseIdQueryHandler : IRequestHandler<GetStudentsByCourseIdQuery, List<string>>
    {
        private readonly ICourseRepository _courseRepository;


        public GetStudentsByCourseIdQueryHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;

        }

        public async Task<List<string>> Handle(GetStudentsByCourseIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(request.Id)
                    ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString());

                var enrollments = await _courseRepository.GetAllEnrollmentAsync();

                var students = enrollments.FindAll(e => e.Course_id == request.Id && e.Status == EnrollmentStatus.Approved)
                    .Select(s => s.Student.FullName);

                return students.ToList();
            }
            catch (ResourceNotFoundException ex)
            {
                throw;
            }

        }
    }
}
