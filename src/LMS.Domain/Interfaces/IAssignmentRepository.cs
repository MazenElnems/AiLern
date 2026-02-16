using LMS.Domain.Entities.Assignments;

namespace LMS.Domain.Repositories;

public interface IAssignmentRepository : IBaseRepository<Assignment>
{
    void DeleteFile(AssignmentFile file);
    List<AssignmentFile> GetFilesByAssignmentId(int assignmentId);
}
