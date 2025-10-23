using LMS.Core.Domain.Entities;
using LMS.Core.DTOs.Course;

namespace LMS.Core.Domain.RepositoriesInterfaces;

public interface ICourseRepository
{
    Task<int> AddAsync(Course course);
    Task<List<Course>> GetAllAsync(); 
}
