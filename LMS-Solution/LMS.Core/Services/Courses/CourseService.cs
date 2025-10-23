using AutoMapper;
using LMS.Core.Domain.Entities;
using LMS.Core.Domain.Enums;
using LMS.Core.Domain.RepositoriesInterfaces;
using LMS.Core.DTOs.Course;
using LMS.Core.Services.Courses.Interfaces;

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

            course.CreatedAt = DateTime.Now;
            course.CourseStatus = CourseStatus.Pending;
            course.InstructorId = instructorId;

            int id = await _courseRepository.AddAsync(course);
            return id;
        }
    }
}
