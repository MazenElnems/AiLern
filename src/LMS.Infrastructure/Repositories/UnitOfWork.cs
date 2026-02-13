using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Repositories.Assignments;
using LMS.Infrastructure.Repositories.Courses;
using LMS.Infrastructure.Repositories.Enrollments;
using LMS.Infrastructure.Repositories.MaterialFiles;
using LMS.Infrastructure.Repositories.RefreshTokens;
using LMS.Infrastructure.Repositories.Sections;
using LMS.Infrastructure.Repositories.Submissions;
using LMS.Infrastructure.Repositories.Users;

namespace LMS.Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public ICourseRepository Courses { get; }

    public IUsersRepository Users { get; }

    public IEnrollmentRepository Enrollments { get; }

    public IRefreshTokenRepository RefreshTokens { get; }

    public IAssignmentRepository Assignments { get; }

    public ISubmissionRepository Submissions { get; }

    public ISectionRepository Sections { get; }
    
    public IMaterialFileRepository MaterialFiles { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Courses = new CourseRepository(_context);
        Users = new UsersRepository(_context);
        Enrollments = new EnrollmentRepository(_context);
        RefreshTokens = new RefreshTokenRepository(_context);
        Assignments = new AssignmentRepository(_context);
        Submissions = new SubmissionRepository(_context);
        Sections = new SectionRepository(_context);
        MaterialFiles = new MaterialFileRepository(_context);
        SubmissionFiles = new BaseRepository<AssignmentSubmissionFile>(_context);
    }

    public async Task<int> CommitAsync() => await _context.SaveChangesAsync();
}
