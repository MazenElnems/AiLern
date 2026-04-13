using LMS.Domain.Entities.Assignments;

namespace LMS.Application.Contracts.Repositories;

public interface IAssignmentRepository : IBaseRepository<Assignment>
{
    void DeleteFile(AssignmentFile file);
    List<AssignmentFile> GetFilesByAssignmentId(int assignmentId);
}
