using AutoMapper;
using LMS.Application.CurrentUser;
using LMS.Domain.DTOs.Assignments;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common.Errors;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.CreateAssignment;

public class AssignmentCreateCommandHandler : IRequestHandler<AssignmentCreateCommand, Result<AssignmentDto>>
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

    public async Task<Result<AssignmentDto>> Handle(AssignmentCreateCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);

        if(course == null) 
            return Result<AssignmentDto>.Failure(DomainErrors.Course.NotFound(request.CourseId));

        if(course.InstructorId != userId)
            return Result<AssignmentDto>.Failure(DomainErrors.Common.Forbidden("You do not have permission to create an assignment for this course."));

        var assignment = _mapper.Map<Assignment>(request);
        
        assignment.CreatedAt = DateTime.UtcNow;
        assignment.IsPublished = false;

        await _unitOfWork.Assignments.InsertAsync(assignment);
        await _unitOfWork.CommitAsync();

        return Result<AssignmentDto>.Success(_mapper.Map<AssignmentDto>(assignment));
    }
}
