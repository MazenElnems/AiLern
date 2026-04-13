using LMS.Application.Features.Dashboards.Shared.DTO;
using LMS.Domain.Entities.Quizzes;

namespace LMS.Application.Contracts.Repositories;

public interface IQuizRepository : IBaseRepository<Quiz>
{
    Task<List<QuizStatisticsDto>> GetQuizStatisticsForCourseAsync(int courseId);
}
