using LMS.Domin.Entities;

namespace LMS.Domin.Repositories;

public interface ICourseRepository : IBaseRepository<Course>
{
    Task<Course?> GetByIdWithDetailsAsync(int id);
    Task<List<Student>> GetStudentsByCourseIdAsync(int courseId);
}
