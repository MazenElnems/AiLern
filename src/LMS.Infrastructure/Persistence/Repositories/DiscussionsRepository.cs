using LMS.Application.Contracts.Repositories;
using LMS.Domain.Entities.CourseDiscussion;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class DiscussionsRepository : BaseRepository<Discussion>, IDiscussionsRepository
{
    private readonly AppDbContext _context;
    public DiscussionsRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
}
