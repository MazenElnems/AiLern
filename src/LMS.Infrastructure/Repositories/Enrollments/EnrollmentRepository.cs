using LMS.Domain.Common.Enums;
using LMS.Domain.DTOs.Courses;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories.Enrollments;

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

    public async Task<List<GetEnrollmentRequestsDto>> GetEnrollmentRequestsAsync(int courseId)
    {
        return await _context.Enrollments
            .Where(e => e.Status == EnrollmentStatus.Pending)
            .Select(e => new GetEnrollmentRequestsDto
            {
                Id = e.Student.Id,
                Name = e.Student.FullName,
                Email = e.Student.Email!,
                StudentId = e.Student.StudentId,
                RequestAt = e.Requested_at
            }).ToListAsync();
    }

    public async Task<bool> IsEnrolledAsync(int courseId, int studentId)
    {
        var Isenrollment = await _context.Enrollments.AnyAsync(e =>e.Course_id == courseId && e.Student_id == studentId);
        return Isenrollment;
    }
}