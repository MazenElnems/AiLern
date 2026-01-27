using LMS.Domain.DTOs.Courses;
using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface IEnrollmentRepository : IBaseRepository<Enrollment>
{
    Task<Enrollment?> GetEnrollmentByIdAsync(int courseId, int studentId);
    Task<List<Enrollment>> GetAllEnrollmentAsync();
    Task<List<GetEnrollmentRequestsDto>> GetEnrollmentRequestsAsync(int courseId);

    Task<bool> IsEnrolledAsync(int courseId, int studentId);
}