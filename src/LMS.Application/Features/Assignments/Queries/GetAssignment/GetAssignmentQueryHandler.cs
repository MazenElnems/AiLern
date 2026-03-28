using AutoMapper;
using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.ConfigurationOptions;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Assignments.Shared.DTO;
using LMS.Domain.Constants;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace LMS.Application.Features.Assignments.Queries.GetAssignment;

public class GetAssignmentQueryHandler : IRequestHandler<GetAssignmentQuery, Result<AssignmentWithFilesDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;
    private readonly IUserContext _userContext;
    private readonly IBunnyUrlSigner _urlSigner;
    private readonly BunnyOptions _bunnyOptions;

    public GetAssignmentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IPermissionService permissionService, IUserContext userContext, IBunnyUrlSigner urlSigner, IOptions<BunnyOptions> bunnyOptions)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _permissionService = permissionService;
        _userContext = userContext;
        _urlSigner = urlSigner;
        _bunnyOptions = bunnyOptions.Value;
    }

    public async Task<Result<AssignmentWithFilesDto>> Handle(GetAssignmentQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var assignmentResult = await _permissionService.AuthorizeAssignmentAccessAsync(request.Id);
        if (!assignmentResult.IsSuccess) return Result<AssignmentWithFilesDto>.Failure(assignmentResult.Error!);
        var assignment = assignmentResult.Value!;

        if (user.IsInRole(UserRoles.Student) && !assignment.IsPublished)
            return DomainErrors.Common.Forbidden("It's not allowed to access this assignment.");

        var files = _unitOfWork.Assignments.GetFilesByAssignmentId(request.Id);

        var assignmentDto = _mapper.Map<AssignmentWithFilesDto>(assignment);
        assignmentDto.FileUrls = files
            .Select(file => _urlSigner.GenerateSignedUrl(_bunnyOptions.BaseUrl, _bunnyOptions.Token, file.StoragePath, TimeSpan.FromMinutes(5)))
            .ToList();

        return assignmentDto;
    }
}
