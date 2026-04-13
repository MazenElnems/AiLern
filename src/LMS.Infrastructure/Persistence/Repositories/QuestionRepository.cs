using LMS.Application.Contracts.Repositories;
using LMS.Domain.Entities.Quizzes;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class QuestionsRepository : BaseRepository<Question>, IQuestionsRepository
{
    private readonly AppDbContext _context;

    public QuestionsRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public Task<List<Guid>> GetQuestionIdsByQuizIdAsync(Guid quizId)
    {
        return _context.Questions
            .Where(q => q.QuizId == quizId)
            .Select(q => q.Id)
            .ToListAsync();
    }
}
