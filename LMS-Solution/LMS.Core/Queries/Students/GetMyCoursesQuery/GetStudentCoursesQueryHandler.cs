using AutoMapper;
using LMS.Core.CurrentUser;
using LMS.Domin.Contracts;
using LMS.Domin.DTOs.Courses;
using LMS.Domin.Entities;
using LMS.Domin.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LMS.Core.Queries.Students.GetMyCoursesQuery;

public class GetStudentCoursesQueryHandler : IRequestHandler<GetStudentCoursesQuery, List<GetStudentCoursesDto>>
{
    private readonly UserManager<ApplicationUser> _user;
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetStudentCoursesQueryHandler> _logger;
    private readonly IUserContext _userContext;

    public GetStudentCoursesQueryHandler(ICourseRepository courseRepository, IMapper mapper, UserManager<ApplicationUser> user, ILogger<GetStudentCoursesQueryHandler> logger, IUserContext userContext)
    {
        _courseRepository = courseRepository;
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

            var courses = await _courseRepository.GetStudentCoursesAsync(
                id,
                request.SearchString,
                request.SortBy,
                request.Order,
                request.PageNumber, 
                request.PageSize
            );

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
