using LMS.Domin.DTOs.Courses;
using LMS.Domin.Entities;

namespace LMS.Domin.Repositories;

public interface IEnrollmentRepository : IBaseRepository<Enrollment>
{
    Task<Enrollment?> GetEnrollmentByIdAsync(int courseId, int studentId);
    Task<List<Enrollment>> GetAllEnrollmentAsync();
    Task<List<GetEnrollmentRequestsDto>> GetEnrollmentRequestsAsync(int courseId);
}