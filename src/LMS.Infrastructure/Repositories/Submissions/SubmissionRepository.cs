using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Data;

namespace LMS.Infrastructure.Repositories.Submissions
{
    internal class SubmissionRepository : BaseRepository<AssignmentSubmission> , ISubmissionRepository
    {
        private readonly AppDbContext _context;
        public SubmissionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
