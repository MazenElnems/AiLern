using LMS.Domain.Entities.Quizzes;

namespace LMS.Application.Contracts.Repositories;

public interface IQuestionsRepository : IBaseRepository<Question>
{
    Task<List<KeyValuePair<Guid, List<Guid>>>> GetQuestionIdsWithOptionIdsByQuizIdAsync(Guid quizId);
}
