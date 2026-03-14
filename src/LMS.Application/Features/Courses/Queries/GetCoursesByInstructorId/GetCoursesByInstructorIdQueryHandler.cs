using AutoMapper;
using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Courses.Shared.DTO;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LMS.Application.Features.Courses.Queries.GetCoursesByInstructorId;

public class GetCoursesByInstructorIdQueryHandler : IRequestHandler<GetCoursesByInstructorIdQuery, Result<List<GetStudentCoursesDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCoursesByInstructorIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<GetStudentCoursesDto>>> Handle(GetCoursesByInstructorIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetAsync(u => u.Id == request.instructorId && u.Role == UserRoles.Instructor);

        if (user == null)
            return DomainErrors.User.NotFound(request.instructorId.ToString());

        var courses = await _unitOfWork.Courses.FilterAsync(c => c.InstructorId == request.instructorId && c.CourseStatus == CourseStatus.Approved,
            includeProperties: [nameof(Course.Instructor)]);

        var result = _mapper.Map<List<GetStudentCoursesDto>>(courses);

        return Result<List<GetStudentCoursesDto>>.Success(result, "Instructor courses retrieved successfully.");



    }
}
