using LMS.Application.Contracts.Repositories;
using LMS.Domain.Entities.Quizzes;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class AnswersRepository : BaseRepository<Answer>, IAnswersRepository
{
    private readonly AppDbContext _context;

    public AnswersRepository(AppDbContext context)
        :base(context)
    {
        _context = context;
    }
}
