using AutoMapper;
using LMS.Application.CurrentUser;
using LMS.Domain.Constants;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Errors;
using LMS.Application.Features.Assignments.Shared.DTO;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Settings;

namespace LMS.Application.Features.Assignments.Queries.GetAssignment;

public class GetAssignmentQueryHandler : IRequestHandler<GetAssignmentQuery, Result<AssignmentWithFilesDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IBunnyUrlSigner _urlSigner;
    private readonly BunnyOptions _bunnyOptions;

    public GetAssignmentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext, IBunnyUrlSigner urlSigner, IOptions<BunnyOptions> bunnyOptions)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
        _urlSigner = urlSigner;
        _bunnyOptions = bunnyOptions.Value;
    }

    public async Task<Result<AssignmentWithFilesDto>> Handle(GetAssignmentQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var assignment = await _unitOfWork.Assignments.GetAsync(a => a.Id == request.Id,
            [nameof(Assignment.Course), nameof(Assignment.Files)]);

        if (assignment == null)
            return DomainErrors.Assignment.NotFound(request.Id);

        var course = assignment.Course;

        if(user.IsInRole(UserRoles.Instructor) && course.InstructorId != user.Id)
            return DomainErrors.Common.Forbidden("You are not the instructor of this course.");

        if(user.IsInRole(UserRoles.Student) && !await _unitOfWork.Enrollments.IsEnrolledAsync(course.Id, user.Id))
            return DomainErrors.Common.Forbidden("You are not enrolled in this course.");

        if(user.IsInRole(UserRoles.Student) && !assignment.IsPublished)
            return DomainErrors.Common.Forbidden("It's not allowed to access this assignment.");

        var assignmentDto = _mapper.Map<AssignmentWithFilesDto>(assignment);
        assignmentDto.FileUrls = assignment.Files
            .Select(file => _urlSigner.GenerateSignedUrl(_bunnyOptions.BaseUrl , _bunnyOptions.Token, file.StoragePath, TimeSpan.FromMinutes(5)))
            .ToList();

        return assignmentDto;
    }
}
