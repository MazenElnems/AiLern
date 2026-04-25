using LMS.Domain.Entities.Quizzes;

namespace LMS.Application.Contracts.Repositories;

public interface IAttemptRepository : IBaseRepository<Attempt>
{
    Task<bool> HasInProgressAttemptAsync(int courseId, int studentId);
}
