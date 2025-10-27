using AutoMapper;
using LMS.Shared.Domain.RepositoriesInterfaces;
using LMS.Shared.Services.Courses.Interfaces;
using LMS.Shared.DTOs.Courses;
using LMS.Core.Domain.Entities;
using LMS.Core.Domain.Enums;
using LMS.Core.Domain.RepositoriesInterfaces;
using LMS.Core.DTOs.Course;
using LMS.Core.Services.Courses.Interfaces;

namespace LMS.Core.Services.Courses;

namespace LMS.Core.Services.Courses
{
internal class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public CourseService(ICourseRepository courseRepository, IMapper mapper)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
    }

    public async Task<int> CreateAsync(CreateCourseDto dto, int instructorId)
    {
        var course = _mapper.Map<Course>(dto);

        course.CreatedAt = DateTime.UtcNow;
            course.CreatedAt = DateTime.Now;
        course.CourseStatus = CourseStatus.Pending;
        course.InstructorId = instructorId;

        int id = await _courseRepository.AddAsync(course);
        return id;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course is null)
            return false;

        await _courseRepository.RemoveAsync(course);
        return true;
    }

    public async Task<List<GetAllCoursesDto>> GetAllCoursesAsync(CouseQueryDto query)
    {
        //var courses = await _courseRepository.GetAllAsync();
        //var dto = _mapper.Map<List<GetAllCoursesDto>>(courses);
        //return dto;
        var courses = await _courseRepository.GetAllAsync(query.SortBy, query.Order, query.Status, query.PageNumber, query.PageSize);
        var dto = _mapper.Map<List<GetAllCoursesDto>>(courses);
        return dto;
    }

    public async Task<GetCourseDto?> GetByIdAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if(course is null)
            return null;

        var dto = _mapper.Map<GetCourseDto>(course);
        return dto;
    }

    public async Task<bool> UpdateAsync(UpdateCourseDto dto)
    {
        var course = await _courseRepository.GetByIdAsync(dto.Id);

        if (course is null)
            return false;

        course.Name = dto.Name;
        course.Description = dto.Description;
        course.Code = dto.Code;

        await _courseRepository.CommitAsync();
        return true;
    }
}
