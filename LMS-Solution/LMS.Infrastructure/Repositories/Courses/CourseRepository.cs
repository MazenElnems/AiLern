using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LMS.Core.Constants;
using LMS.Domin.RepositoriesInterfaces;
using LMS.Domin.Entities;

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

    public async Task<List<Course>> GetAllAsync(string sortBy, string order, string? status, int pageNo = 1, int pageSize = 10)
    {
        IQueryable<Course> query = _db.Courses;

        // 1) Filter
        // 2) Sorting
        // 3) Pagination

        if (status != null)
            query = query.Where(c => c.CourseStatus.ToString().ToLower() == status.ToLower()).Select(c => new Course
            {
                Id = c.Id,
                Description = c.Description,
                Name = c.Name,
                Code = c.Code,
                CourseStatus = c.CourseStatus,
                ApprovedDate = c.ApprovedDate,
                CreatedAt = c.CreatedAt,
                InstructorId = c.InstructorId,
                Instructor = new Instructor { UserName = c.Instructor.UserName },
                Approvedby = c.Approvedby,
                SectionCourseId = c.SectionCourseId,
                Section = new Course { Name = (c.Section == null ? null : c.Section.Name)! },
                Admin = new Admin { UserName = c.Admin == null ? null : c.Admin.UserName }
            });

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
    
    public async Task<Course?> GetByIdAsync(int id)
    {
        return await _db.Courses
            .Select(c => new Course
            {
                Id = c.Id,
                Description = c.Description,
                Name = c.Name,
                Code = c.Code,
                CourseStatus = c.CourseStatus,
                ApprovedDate = c.ApprovedDate,
                CreatedAt = c.CreatedAt,
                InstructorId = c.InstructorId,
                Instructor = new Instructor { UserName = c.Instructor.UserName },
                Approvedby = c.Approvedby,
                SectionCourseId = c.SectionCourseId,
                Section = new Course { Name = (c.Section == null ? null : c.Section.Name)! },
                Admin = new Admin { UserName = c.Admin == null ? null : c.Admin.UserName }
            })
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> RemoveAsync(Course course)
    {
        _db.Remove(course);
        return await _db.SaveChangesAsync();
    }
}
