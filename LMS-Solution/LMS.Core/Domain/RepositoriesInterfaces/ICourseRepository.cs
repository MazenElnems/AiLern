using LMS.Core.Domain.Entities;

namespace LMS.Core.Domain.RepositoriesInterfaces;

public interface ICourseRepository
{
    Task<int> AddAsync(Course course);
}
