using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class AttemptAnswersRepository : BaseRepository<AttemptAnswer>, IAttemptAnswersRepository
{
    private readonly AppDbContext _context;

    public AttemptAnswersRepository(AppDbContext context)
        :base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AttemptAnswer>> GetAttemptAnswersByIdAsync(Guid attemptId)
    {
        return await _context.AttemptAnswers
            .Where(a => a.AttemptId == attemptId)
            .Include(a => a.Question)
            .ToListAsync();
    }
}
