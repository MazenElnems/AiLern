using LMS.Domin.Entities;
using System.Linq.Expressions;

namespace LMS.Domin.Contracts;

public interface ICourseRepository
{
    Task<int> AddAsync(Course course);
    Task<Course?> GetByIdAsync(int id);
    Task<Course?> GetByIdWithDetailsAsync(int id);
    Task<List<Course>> GetPagedCourses(string searchString,string sortBy, string order, int pageNo = 1, int pageSize = 10); 
    Task<List<Course>> GetPagedCoursesWithFilterAsync(Expression<Func<Course, bool>> filter, string searchString, string sortBy, string order, int pageNo = 1, int pageSize = 10); 
    Task<int> RemoveAsync(Course course);
    Task<int> CommitAsync();
    Task<Enrollment> GetEnrollmentByIdAsync(int courseId, int studentId);
    Task<List<Enrollment>> GetAllEnrollmentAsync();

}
