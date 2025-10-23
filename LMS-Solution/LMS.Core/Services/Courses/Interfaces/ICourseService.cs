using LMS.Core.DTOs.Course;

namespace LMS.Core.Services.Courses.Interfaces;

public interface ICourseService
{
    Task<int> CreateAsync(CreateCourseDto dto, int instructorId);
}
