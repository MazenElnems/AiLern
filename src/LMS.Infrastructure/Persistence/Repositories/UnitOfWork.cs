using LMS.Application.Contracts.Repositories;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Quizzes;

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
    public IBaseRepository<AIQuestionGenerationJob> QuestionGenerationJobs { get; }
    public IBaseRepository<QuestionGenerationFiles> QuestionGenerationFiles { get; }
    public IBaseRepository<Attempt> Attempts { get; }
    public IAnswersRepository Answers { get; }

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
        QuestionGenerationJobs = new BaseRepository<AIQuestionGenerationJob>(_context);
        QuestionGenerationFiles = new BaseRepository<QuestionGenerationFiles>(_context);
        Attempts = new BaseRepository<Attempt>(_context);
        Answers = new AnswersRepository(_context);
    }

    public async Task<int> CommitAsync() => await _context.SaveChangesAsync();

    public Task<int> CommitAsync(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);
}

