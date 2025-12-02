using AutoMapper;
using LMS.Core.DTOs.Courses;
using LMS.Domin.Contracts;
using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Queries.Students.GetAllQuery
{
    public class GetStudentCoursesQueryHandler : IRequestHandler<GetStudentCoursesQuery, List<GetStudentCoursesDto>>
    {
        private readonly UserManager<ApplicationUser> _user;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetStudentCoursesQueryHandler> _logger;

        public GetStudentCoursesQueryHandler(ICourseRepository courseRepository, IMapper mapper, UserManager<ApplicationUser> user, ILogger<GetStudentCoursesQueryHandler> logger)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
            _user = user;
            _logger = logger;
        }

        public async Task<List<GetStudentCoursesDto>> Handle(GetStudentCoursesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _user.FindByIdAsync(request.Id.ToString())
                    ?? throw new ResourceNotFoundException(nameof(ApplicationUser), request.Id.ToString());


                var courses = await _courseRepository.GetStudentCoursesAsync(request.Id, request.SearchString,
                    request.SortBy, request.Order, request.PageNumber, request.PageSize)
                    ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString()); ;

                var dto = _mapper.Map<List<GetStudentCoursesDto>>(courses);
                return dto;
            }
            catch (ResourceNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while get courses for student ID {StudentId}",request.Id);
                throw;

            }

        }
    }
}
