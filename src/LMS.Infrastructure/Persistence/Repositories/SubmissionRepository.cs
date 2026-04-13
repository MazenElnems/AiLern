using LMS.Application.Contracts.Repositories;
using LMS.Domain.Entities.Assignments;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class SubmissionRepository : BaseRepository<AssignmentSubmission> , ISubmissionRepository
{
    private readonly AppDbContext _context;
    public SubmissionRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
