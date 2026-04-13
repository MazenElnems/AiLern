using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Users;

namespace LMS.Application.Contracts.Repositories;

public interface ICourseRepository : IBaseRepository<Course>
{
    Task<Course?> GetByIdWithDetailsAsync(int id);
    Task<List<Student>> GetStudentsByCourseIdAsync(int courseId);
}
