using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Repositories.MaterialFiles;
using LMS.Infrastructure.Repositories.Sections;

namespace LMS.Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public ICourseRepository Courses { get; }
    public IUsersRepository Users { get; }
    public IEnrollmentRepository Enrollments { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public IAssignmentRepository Assignments { get; }
    public IBaseRepository<AssignmentSubmissionFile> SubmissionFiles { get; }
    public ISectionRepository Sections { get; }
    public IMaterialFileRepository MaterialFiles { get; }
    public IBaseRepository<AssignmentSubmission> AssignmentSubmissions { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Courses = new CourseRepository(_context);
        Users = new UsersRepository(_context);
        Enrollments = new EnrollmentRepository(_context);
        RefreshTokens = new RefreshTokenRepository(_context);
        Assignments = new AssignmentRepository(_context);
        Sections = new SectionRepository(_context);
        MaterialFiles = new MaterialFileRepository(_context);
        SubmissionFiles = new BaseRepository<AssignmentSubmissionFile>(_context);
        AssignmentSubmissions = new BaseRepository<AssignmentSubmission>(_context); 
    }

    public async Task<int> CommitAsync() => await _context.SaveChangesAsync();
}

