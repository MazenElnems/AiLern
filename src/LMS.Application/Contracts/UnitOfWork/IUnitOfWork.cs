using LMS.Application.Contracts.Repositories;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Notification;
using LMS.Domain.Entities.Quizzes;

namespace LMS.Application.Contracts.UnitOfWork;

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
    public IQuizRepository Quizzes { get; }
    public IQuestionsRepository Questions { get; }
    public IBaseRepository<AIQuestionGenerationJob> QuestionGenerationJobs { get; }
    public IBaseRepository<QuestionGenerationFiles> QuestionGenerationFiles { get; }
    public IAttemptRepository Attempts { get; }
    public IAnswersRepository Answers { get; }
    public IBaseRepository<CourseProgress> CourseProgress { get; }
    public IBaseRepository<SectionProgress> SectionProgress { get;  }       
    public IBaseRepository<Notification> Notifications { get; }
    public IBaseRepository<UserNotification> UserNotifications { get; }

    Task<int> CommitAsync();
    Task<int> CommitAsync(CancellationToken cancellationToken);
}
