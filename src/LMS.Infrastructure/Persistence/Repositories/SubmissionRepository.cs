using LMS.Domain.Entities.Assignments;
using LMS.Domain.Repositories;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class SubmissionRepository : BaseRepository<AssignmentSubmission> , ISubmissionRepository
{
    private readonly AppDbContext _context;
    public SubmissionRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
