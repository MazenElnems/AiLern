using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface ICourseRepository : IBaseRepository<Course>
{
    Task<Course?> GetByIdWithDetailsAsync(int id);
    Task<List<Student>> GetStudentsByCourseIdAsync(int courseId);
}
