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

namespace LMS.Core.Queries.Courses.GetApprovedQueries
{
    public class GetApprovedCoursesQueryHandler : IRequestHandler<GetApprovedCoursesQuery, List<GetApprovedCoursesDto>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public GetApprovedCoursesQueryHandler(ICourseRepository courseRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<List<GetApprovedCoursesDto>> Handle(GetApprovedCoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await _courseRepository.GetAllAsync(request.SortBy, request.Order, CourseStatus.Approved.ToString(), request.PageNumber, request.PageSize);
            var dto = _mapper.Map<List<GetApprovedCoursesDto>>(courses);
            return dto;
        }
    }
}
