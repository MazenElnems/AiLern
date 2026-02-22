using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Assignments.DTO;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Assignments.Queries.GetCourseAssignmentsForInstructors;

public class GetCourseAssignmentsForInstructorsQueryHandler : IRequestHandler<GetCourseAssignmentsForInstructorsQuery, Result<List<GetAllAssignmentForInstructorDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public GetCourseAssignmentsForInstructorsQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<Result<List<GetAllAssignmentForInstructorDto>>> Handle(GetCourseAssignmentsForInstructorsQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var course = await _unitOfWork.Courses.GetAsync(c => c.Id == request.CourseId,
            includeProperties: [nameof(Course.Assignments)]);

        if (course == null)
            return DomainErrors.Course.NotFound(request.CourseId);

        if (course.InstructorId != user.Id)
            return DomainErrors.Course.NotOwned;

        var assignments = course.Assignments;

        return _mapper.Map<List<GetAllAssignmentForInstructorDto>>(assignments);
    }
}