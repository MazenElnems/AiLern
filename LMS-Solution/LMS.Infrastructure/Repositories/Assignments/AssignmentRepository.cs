using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Data;

namespace LMS.Infrastructure.Repositories.Assignments;

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
