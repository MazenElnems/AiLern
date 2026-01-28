using AutoMapper;
using LMS.Core.CurrentUser;
using LMS.Domain.DTOs.Assignments;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Core.Commands.Assignments.AssignmentCreateCommands;

public class AssignmentCreateCommandHandler : IRequestHandler<AssignmentCreateCommand, AssignmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public AssignmentCreateCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<AssignmentDto> Handle(AssignmentCreateCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);

        if(course == null) 
            throw new ResourceNotFoundException(nameof(Course), request.CourseId.ToString());

        if(course.InstructorId != userId)
            throw new UnauthorizedAccessException("You do not have permission to create an assignment for this course.");

        var assignment = _mapper.Map<Assignment>(request);
        
        assignment.CreatedAt = DateTime.UtcNow;
        assignment.IsPublished = false;

        await _unitOfWork.Assignments.InsertAsync(assignment);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<AssignmentDto>(assignment);
    }
}
