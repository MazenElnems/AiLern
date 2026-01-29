using AutoMapper;
using LMS.Core.CurrentUser;
using LMS.Domain.Common.Enums;
using LMS.Domain.Constants;
using LMS.Domain.DTOs.Assignments;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Core.Commands.Assignments.GetAssignmentCommands;

public class GetAssignmentCommandHandler : IRequestHandler<GetAssignmentCommand, AssignmentWithFilesDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetAssignmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<AssignmentWithFilesDto> Handle(GetAssignmentCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses
            .GetAsync(c => c.Id == request.CourseId, [nameof(Course.Assignments), nameof(Course.Enrollments)]);

        if (course == null)
            throw new ResourceNotFoundException(nameof(Course), request.CourseId.ToString());

        var assignment = course.Assignments.FirstOrDefault(a => a.Id == request.Id);

        if (assignment == null)
            throw new ResourceNotFoundException(nameof(Assignment), request.Id.ToString());

        var user = _userContext.GetCurrentUser();

        if(user.IsInRole(UserRoles.Student)) 
        {
            var isEnrolled = course.Enrollments.Any(e => e.Student_id == user.Id && e.Status == EnrollmentStatus.Approved);
            if (!isEnrolled)
                throw new UnauthorizedAccessException("You are not enrolled in this course.");
        }

        if(user.IsInRole(UserRoles.Student) && !assignment.IsPublished)
            throw new UnauthorizedAccessException("it's not allowed to access this assignment.");

        if (user.IsInRole(UserRoles.Instructor)) 
        {
            if(course.InstructorId != user.Id) 
            {
                throw new UnauthorizedAccessException("You are not the instructor of this course.");
            }
        }

        var assignmentFiles = _unitOfWork.Assignments.GetFilesByAssignmentId(assignment.Id);
        var assignmentDto = _mapper.Map<AssignmentWithFilesDto>(assignment);
        assignmentDto.FilePaths = assignmentFiles.Select(f => f.StoragePath).ToList();

        return assignmentDto;
    }
}
