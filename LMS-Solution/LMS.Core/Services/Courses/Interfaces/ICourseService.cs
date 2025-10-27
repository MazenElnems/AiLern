using LMS.Shared.DTOs.Courses;
using LMS.Core.DTOs.Course;

namespace LMS.Shared.Services.Courses.Interfaces;
namespace LMS.Core.Services.Courses.Interfaces;

public interface ICourseService
{
    Task<int> CreateAsync(CreateCourseDto dto, int instructorId);
    Task<List<GetAllCoursesDto>> GetAllCoursesAsync(CouseQueryDto query);
    Task<GetCourseDto?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateAsync(UpdateCourseDto dto);
}
