using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Repositories;

namespace LMS.Domain.Interfaces;

public interface IQuestionsRepository : IBaseRepository<Question>
{
    Task<List<Guid>> GetQuestionIdsByQuizIdAsync(Guid quizId);
}
