using AutoMapper;
using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Assignments.Shared.DTO;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Assignments.Queries.GetCourseAssignmentsForInstructors;

public class GetCourseAssignmentsForInstructorsQueryHandler : IRequestHandler<GetCourseAssignmentsForInstructorsQuery, Result<List<GetAllAssignmentForInstructorDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;

    public GetCourseAssignmentsForInstructorsQueryHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _mapper = mapper;
    }

    public async Task<Result<List<GetAllAssignmentForInstructorDto>>> Handle(GetCourseAssignmentsForInstructorsQuery request, CancellationToken cancellationToken)
    {
        var courseResult = await _permissionService.AuthorizeInstructorAccessToCourseAsync(request.CourseId);
        if (!courseResult.IsSuccess) return Result<List<GetAllAssignmentForInstructorDto>>.Failure(courseResult.Error!);

        var assignments = await _unitOfWork.Assignments.FilterAsync(a => a.CourseId == request.CourseId);

        return _mapper.Map<List<GetAllAssignmentForInstructorDto>>(assignments);
    }
}
