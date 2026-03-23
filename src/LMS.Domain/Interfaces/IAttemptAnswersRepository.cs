using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Repositories;

namespace LMS.Domain.Interfaces;

public interface IAttemptAnswersRepository : IBaseRepository<AttemptAnswer>
{
    Task<IEnumerable<AttemptAnswer>> GetAttemptAnswersByIdAsync(Guid attemptId);
}
