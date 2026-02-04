using LMS.Domain.Entities;

namespace LMS.Domain.Repositories;

public interface IUnitOfWork
{
    public ICourseRepository Courses { get; }
    public IUsersRepository Users { get; }
    public IEnrollmentRepository Enrollments { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public IAssignmentRepository Assignments { get; }
    public ISubmissionRepository Submissions { get; }
    public IBaseRepository<AssignmentSubmissionFile> SubmissionFiles { get; }
    Task<int> CommitAsync();
}
