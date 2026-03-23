using LMS.Domain.Entities.Assignments;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Quizzes;

namespace LMS.Domain.Repositories;

public interface IUnitOfWork
{
    public ICourseRepository Courses { get; }
    public IUsersRepository Users { get; }
    public IEnrollmentRepository Enrollments { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public IAssignmentRepository Assignments { get; }
    public IBaseRepository<AssignmentSubmissionFile> SubmissionFiles { get; }
    public IBaseRepository<AssignmentSubmission> AssignmentSubmissions { get; }
    public IBaseRepository<Section> Sections { get; }
    public IBaseRepository<Quiz> Quizzes { get; }
    public IBaseRepository<Question> Questions { get; }
    public IBaseRepository<AIQuestionGenerationJob> QuestionGenerationJobs { get; }
    public IBaseRepository<QuestionGenerationFiles> QuestionGenerationFiles { get; }
    public IBaseRepository<Attempt> Attempts { get; }
    Task<int> CommitAsync();
}
