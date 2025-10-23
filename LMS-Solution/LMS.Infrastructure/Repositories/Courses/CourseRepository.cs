using LMS.Core.Domain.Entities;
using LMS.Core.Domain.RepositoriesInterfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<List<Course>> GetAllAsync()
    {
        return await _db.Courses
            .Select(c => new Course
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                CourseStatus = c.CourseStatus,  
                ApprovedDate = c.ApprovedDate,
                CreatedAt = c.CreatedAt,
                InstructorId = c.InstructorId,  
                Instructor = new Instructor { UserName = c.Instructor.UserName},
                Approvedby = c.Approvedby,
                SectionCourseId = c.SectionCourseId,
                Section = new Course { Name = (c.Section == null ? null : c.Section.Name)! }
            })
            .ToListAsync();
    }
}
