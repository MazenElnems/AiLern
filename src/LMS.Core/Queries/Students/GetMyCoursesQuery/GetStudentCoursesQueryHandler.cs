using AutoMapper;
using LMS.Core.CurrentUser;
using LMS.Domain.Constants;
using LMS.Domain.Repositories;
using LMS.Domain.DTOs.Courses;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using LMS.Domain.Common.Enums;

namespace LMS.Core.Queries.Students.GetMyCoursesQuery;

public class GetStudentCoursesQueryHandler : IRequestHandler<GetStudentCoursesQuery, List<GetStudentCoursesDto>>
{
    private readonly UserManager<ApplicationUser> _user;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStudentCoursesQueryHandler> _logger;
    private readonly IUserContext _userContext;

    public GetStudentCoursesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> user, ILogger<GetStudentCoursesQueryHandler> logger, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _user = user;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<List<GetStudentCoursesDto>> Handle(GetStudentCoursesQuery request, CancellationToken cancellationToken)
    {
        int id = 0;
        try
        {
            id = _userContext.GetCurrentUser().Id;

            var user = await _user.FindByIdAsync(id.ToString())
                ?? throw new ResourceNotFoundException(nameof(ApplicationUser), id.ToString());

            var searchString = request.SearchString;
            Expression<Func<Course, bool>> predicate = c =>
                c.Enrollments.Any(s => s.Status == EnrollmentStatus.Approved && s.Student_id == id);

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                predicate = c =>
                    c.Enrollments.Any(s => s.Status == EnrollmentStatus.Approved && s.Student_id == id)
                    && (c.Name.Contains(searchString) || c.Code.Contains(searchString));
            }

            var sortBy = request.SortBy?.ToLower();
            var order = request.Order?.ToLower();
            var isDescending = order != SortOrderOptions.ASC;

            Expression<Func<Course, object>> orderBy = sortBy switch
            {
                CourseSortByOptions.Name => c => c.Name,
                CourseSortByOptions.CreatedAt => c => c.CreatedAt,
                _ => c => c.CreatedAt
            };

            var courses = await _unitOfWork.Courses.FilterAsync(
                predicate,
                orderBy,
                isDescending,
                (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                new[] { "Instructor" });

            var dto = _mapper.Map<List<GetStudentCoursesDto>>(courses);
            return dto;
        }
        catch (ResourceNotFoundException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while get courses for student ID {StudentId}",id);
            throw;

        }

    }
}
