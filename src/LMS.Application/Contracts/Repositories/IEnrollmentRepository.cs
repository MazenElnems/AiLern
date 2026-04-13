using LMS.Domain.Entities.Courses;

namespace LMS.Application.Contracts.Repositories;

public interface IEnrollmentRepository : IBaseRepository<Enrollment>
{
    Task<Enrollment?> GetEnrollmentByIdAsync(int courseId, int studentId);
    Task<List<Enrollment>> GetAllEnrollmentAsync();
    Task<bool> IsEnrolledAsync(int courseId, int studentId);
}