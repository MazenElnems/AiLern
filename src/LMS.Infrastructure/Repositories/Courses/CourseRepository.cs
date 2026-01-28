using LMS.Domain.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories.Courses;

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
            .Where(s => s.Enrollments.Any(e => e.Course_id == courseId && e.Status == EnrollmentStatus.Approved))
            .ToListAsync();
    }

}
