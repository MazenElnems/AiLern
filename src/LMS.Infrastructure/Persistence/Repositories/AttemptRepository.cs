using LMS.Application.Contracts.Repositories;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class AttemptRepository : BaseRepository<Attempt>, IAttemptRepository
{
    private readonly AppDbContext _context;

    public AttemptRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<bool> HasInProgressAttemptAsync(int courseId, int studentId)
    {
        return await _context.Attempts.AnyAsync(a => 
            a.Status == AttemptStatus.InProgress &&
            a.StudentId == studentId &&
            a.Quiz.CourseId == courseId
        );
    }
}
