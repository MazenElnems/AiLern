using LMS.Application.Contracts.Repositories;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Entities.CourseDiscussion;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Notification;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public ICourseRepository Courses { get; }
    public IUsersRepository Users { get; }
    public IEnrollmentRepository Enrollments { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public IAssignmentRepository Assignments { get; }
    public IBaseRepository<AssignmentSubmissionFile> SubmissionFiles { get; }
    public IBaseRepository<Section> Sections { get; }
    public IBaseRepository<AssignmentSubmission> AssignmentSubmissions { get; }
    public IQuizRepository Quizzes { get; }
    public IQuestionsRepository Questions { get; }
    public IAttemptRepository Attempts { get; }
    public IAnswersRepository Answers { get; }
    public IBaseRepository<CourseProgress> CourseProgress { get; }
    public IBaseRepository<SectionProgress> SectionProgress { get; }
    public IBaseRepository<UserNotification> UserNotifications { get; }
    public IBaseRepository<AIResource> AIResources { get; }
    public IBaseRepository<Notification> Notfications { get; }
    public IBaseRepository<Discussion> Discussions { get; }
    public IBaseRepository<DiscussionVote> DiscussionVotes { get; }


    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Courses = new CourseRepository(_context);
        Users = new UsersRepository(_context);
        Enrollments = new EnrollmentRepository(_context);
        RefreshTokens = new RefreshTokenRepository(_context);
        Assignments = new AssignmentRepository(_context);
        Sections = new BaseRepository<Section>(_context);
        SubmissionFiles = new BaseRepository<AssignmentSubmissionFile>(_context);
        AssignmentSubmissions = new BaseRepository<AssignmentSubmission>(_context);
        Quizzes = new QuizRepository(_context);
        Questions = new QuestionsRepository(_context);
        Attempts = new AttemptRepository(_context);
        Answers = new AnswersRepository(_context);
        CourseProgress = new BaseRepository<CourseProgress>(_context);
        SectionProgress = new BaseRepository<SectionProgress>(_context);
        Notfications = new BaseRepository<Notification>(_context);
        UserNotifications = new BaseRepository<UserNotification>(_context);
        AIResources = new BaseRepository<AIResource>(_context);
        Discussions = new BaseRepository<Discussion>(_context);
        DiscussionVotes = new BaseRepository<DiscussionVote>(_context);
    }

    public async Task<int> CommitAsync() => await _context.SaveChangesAsync();

    public Task<int> CommitAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);
}
