using LMS.Domain.Entities.Assignments;
using LMS.Domain.Enums;
using LMS.Domain.Repositories;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class AssignmentRepository : BaseRepository<Assignment>, IAssignmentRepository
{
    private readonly AppDbContext _context;

    public AssignmentRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public void DeleteFile(AssignmentFile file)
    {
        _context.Set<AssignmentFile>().Remove(file);
    }

    public List<AssignmentFile> GetFilesByAssignmentId(int assignmentId)
    {
        return _context.AssignmentFiles
            .Where(af => af.AssignmentId == assignmentId && af.UploadStatus == UploadStatus.Completed)
            .ToList();
    }
}
