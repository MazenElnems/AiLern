using AutoMapper;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Enums;
using LMS.Domain.Constants;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common.Errors;
using LMS.Domain.DTOs.Assignments;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Assignments.Queries.GetAssignment;

public class GetAssignmentQueryHandler : IRequestHandler<GetAssignmentQuery, Result<AssignmentWithFilesDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetAssignmentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<AssignmentWithFilesDto>> Handle(GetAssignmentQuery request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses
            .GetAsync(c => c.Id == request.CourseId, [nameof(Course.Assignments), nameof(Course.Enrollments)]);

        if (course == null)
            return Result<AssignmentWithFilesDto>.Failure(DomainErrors.Course.NotFound(request.CourseId));

        var assignment = course.Assignments.FirstOrDefault(a => a.Id == request.Id);

        if (assignment == null)
            return Result<AssignmentWithFilesDto>.Failure(DomainErrors.Assignment.NotFound(request.Id));

        var user = _userContext.GetCurrentUser();

        if(user.IsInRole(UserRoles.Student)) 
        {
            var isEnrolled = course.Enrollments.Any(e => e.Student_id == user.Id && e.Status == EnrollmentStatus.Approved);
            if (!isEnrolled)
                return Result<AssignmentWithFilesDto>.Failure(DomainErrors.Common.Forbidden("You are not enrolled in this course."));
        }

        if(user.IsInRole(UserRoles.Student) && !assignment.IsPublished)
            return Result<AssignmentWithFilesDto>.Failure(DomainErrors.Common.Forbidden("It's not allowed to access this assignment."));

        if (user.IsInRole(UserRoles.Instructor)) 
        {
            if(course.InstructorId != user.Id) 
            {
                return Result<AssignmentWithFilesDto>.Failure(DomainErrors.Common.Forbidden("You are not the instructor of this course."));
            }
        }

        var assignmentFiles = _unitOfWork.Assignments.GetFilesByAssignmentId(assignment.Id);
        var assignmentDto = _mapper.Map<AssignmentWithFilesDto>(assignment);
        assignmentDto.FilePaths = assignmentFiles.Select(f => f.StoragePath).ToList();

        return Result<AssignmentWithFilesDto>.Success(assignmentDto);
    }
}
