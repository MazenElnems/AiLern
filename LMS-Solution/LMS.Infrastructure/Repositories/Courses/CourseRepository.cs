using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LMS.Core.Constants;
using LMS.Domin.Entities;
using System.Linq.Expressions;
using LMS.Domin.Contracts;

namespace LMS.Infrastructure.Repositories.Courses;

internal class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _db;

    public CourseRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int> AddAsync(Course course)
    {
        _db.Add(course);
        await CommitAsync();
        return course.Id;
    }

    public async Task<int> CommitAsync()
    {
        return await _db.SaveChangesAsync();
    }

    public async Task<List<Course>> GetPagedCourses(string searchString,string sortBy, string order, int pageNo = 1, int pageSize = 10)
    {
        return await GetPagedCoursesWithFilterAsync(
            c => true,
            searchString,
            sortBy,
            order,
            pageNo,
            pageSize
        );
    }

    public async Task<List<Course>> GetPagedCoursesWithFilterAsync(Expression<Func<Course, bool>> filter, string searchString, string sortBy, string order, int pageNo = 1, int pageSize = 10)
    {
        IQueryable<Course> query = _db.Courses
            .Include(i => i.Instructor);

        // 1) Filter
        // 2) Sorting
        // 3) Pagination

        if (!string.IsNullOrEmpty(searchString))
        {
            var lowerSearch = searchString.ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(lowerSearch) || c.Description.ToLower().Contains(lowerSearch));
        }

        if (filter != null)
            query = query.Where(filter);

        if (sortBy != null && order != null)
        {
            query = (sortBy.ToLower(), order.ToLower()) switch
            {
                (CourseSortByOptions.Name, SortOrderOptions.ASC) => query.OrderBy(c => c.Name),
                (CourseSortByOptions.Name, SortOrderOptions.DESC) => query.OrderByDescending(c => c.Name),
                (CourseSortByOptions.CreatedAt, SortOrderOptions.ASC) => query.OrderBy(c => c.CreatedAt),
                (CourseSortByOptions.CreatedAt, SortOrderOptions.DESC) => query.OrderByDescending(c => c.CreatedAt),
                _ => query
            };
        }

        query = query
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize);

        return await query.ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(int id) =>
        await _db.Courses.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Course?> GetByIdWithDetailsAsync(int id) =>
        await _db.Courses
            .Include(c => c.Instructor)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<int> RemoveAsync(Course course)
    {
        _db.Remove(course);
        return await _db.SaveChangesAsync();
    }

    public async Task<Enrollment?> GetEnrollmentByIdAsync(int courseId, int studentId)
    {
        var enrollment = await _db.Enrollments.FindAsync(courseId, studentId);
        return enrollment;
    }

    public async Task<int> RemoveEnrollmentAsync(Enrollment enrollment)
    {
        _db.Remove(enrollment);
        return await _db.SaveChangesAsync();
    }

    public async Task<List<Enrollment>> GetAllEnrollmentAsync()
    {
        return await _db.Enrollments
            .Include(s =>s.Student)
            .ToListAsync(); 
    }
}
