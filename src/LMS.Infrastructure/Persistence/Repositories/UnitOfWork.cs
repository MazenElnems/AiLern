using LMS.Domain.Entities.Assignments;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;

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
    public IMaterialFileRepository MaterialFiles { get; }
    public IBaseRepository<AssignmentSubmission> AssignmentSubmissions { get; }

    public IBaseRepository<Quiz> Quizzes { get; }
    public IBaseRepository<Question> Questions { get; }
    public IBaseRepository<AIQuestionGenerationJob> QuestionGenerationJobs { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Courses = new CourseRepository(_context);
        Users = new UsersRepository(_context);
        Enrollments = new EnrollmentRepository(_context);
        RefreshTokens = new RefreshTokenRepository(_context);
        Assignments = new AssignmentRepository(_context);
        Sections = new BaseRepository<Section>(_context);
        MaterialFiles = new MaterialFileRepository(_context);
        SubmissionFiles = new BaseRepository<AssignmentSubmissionFile>(_context);
        AssignmentSubmissions = new BaseRepository<AssignmentSubmission>(_context);
        Quizzes = new BaseRepository<Quiz>(_context);
        Questions = new BaseRepository<Question>(_context);
        QuestionGenerationJobs = new BaseRepository<AIQuestionGenerationJob>(_context);
    }

    public async Task<int> CommitAsync() => await _context.SaveChangesAsync();
}

