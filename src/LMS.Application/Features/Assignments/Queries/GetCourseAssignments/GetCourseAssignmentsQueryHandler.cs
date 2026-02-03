using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Errors;
using LMS.Domain.Constants;
using LMS.Domain.DTOs.Assignments;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Assignments.Queries.GetCourseAssignments;

public class GetCourseAssignmentsQueryHandler : IRequestHandler<GetCourseAssignmentsQuery, Result<List<GetAllAssignmentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public GetCourseAssignmentsQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<Result<List<GetAllAssignmentDto>>> Handle(GetCourseAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var course = await _unitOfWork.Courses.GetAsync(c => c.Id == request.CourseId,
            includeProperties: [nameof(Course.Assignments)]);

        if (course == null)
            return DomainErrors.Course.NotFound(request.CourseId);

        var assignments = user.IsInRole(UserRoles.Student)
            ? course.Assignments.Where(a => a.IsPublished)
            : course.Assignments;

        if(user.IsInRole(UserRoles.Student) && !await _unitOfWork.Enrollments.IsEnrolledAsync(course.Id, user.Id))
            return DomainErrors.Common.Forbidden("You are not enrolled in this course.");

        if(user.IsInRole(UserRoles.Instructor) && course.InstructorId != user.Id)
            return DomainErrors.Common.Forbidden("You are not the instructor of this course.");

        var dto = _mapper.Map<List<GetAllAssignmentDto>>(assignments);

        return dto;
    }
}