using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class EnrollmentRepository : BaseRepository<Enrollment>, IEnrollmentRepository
{
    private readonly AppDbContext _context;

    public EnrollmentRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Enrollment?> GetEnrollmentByIdAsync(int courseId, int studentId)
    {
        var enrollment = await _context.Enrollments.FindAsync(courseId, studentId);
        return enrollment;
    }

    public async Task<List<Enrollment>> GetAllEnrollmentAsync()
    {
        return await _context.Enrollments
            .Include(s => s.Student)
            .ToListAsync();
    }

    public async Task<bool> IsEnrolledAsync(int courseId, int studentId)
    {
        var Isenrollment = await _context.Enrollments.AnyAsync(e => e.CourseId == courseId);
        return Isenrollment;
    }
}