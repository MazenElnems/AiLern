using Microsoft.EntityFrameworkCore;
using LMS.Domain.Entities.Users;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Application.Contracts.Repositories;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context) 
        : base(context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdWithDetailsAsync(int id) =>
        await _context.Courses
            .Include(c => c.Instructor)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<List<Student>> GetStudentsByCourseIdAsync(int courseId)
    {
        return await _context.Students
            .Where(s => s.Enrollments.Any(e => e.CourseId == courseId ))
            .ToListAsync();
    }

}
