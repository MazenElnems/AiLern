using LMS.Domin.Entities;

namespace LMS.Domin.RepositoriesInterfaces;

public interface ICourseRepository
{
    Task<int> AddAsync(Course course);
    Task<List<Course>> GetAllAsync(string sortBy, string order, string? status, int pageNo = 1, int pageSize = 10); 
    Task<Course?> GetByIdAsync(int id);
    Task<int> RemoveAsync(Course course);
    Task<int> CommitAsync();
}
