using LMS.Core.Domain.Entities;
using LMS.Core.Domain.RepositoriesInterfaces;
using LMS.Infrastructure.Data;

namespace LMS.Infrastructure.Repositories.Courses;

internal class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _db;

    public CourseRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int> AddAsync(Course course)
    {
        _db.Add(course);
        await CommitAsync();
        return course.Id;
    }

    public async Task<int> CommitAsync()
    {
        return await _db.SaveChangesAsync();
    }
}
