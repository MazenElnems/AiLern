using AutoMapper;
using LMS.Core.DTOs.Courses;
using LMS.Domin.Enums;
using LMS.Domin.RepositoriesInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Queries.Courses.GetPendingQueries
{
    
    public class GetPendingCoursesQueryHandler : IRequestHandler<GetPendingCoursesQuery, List<GetCourseDto>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public GetPendingCoursesQueryHandler(ICourseRepository courseRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<List<GetCourseDto>> Handle(GetPendingCoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await _courseRepository.GetAllAsync(request.SortBy, request.Order, CourseStatus.Pending.ToString(), request.PageNumber, request.PageSize);
            var dto = _mapper.Map<List<GetCourseDto>>(courses);
          
            return dto;
        }
    }
}
