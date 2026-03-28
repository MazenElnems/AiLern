using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.ConfigurationOptions;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace LMS.Application.Features.AssignmentSubmissions.Queries.GetSubmissionFiles;

public class GetAssignmentSubmissionFilesQueryHandler : IRequestHandler<GetAssignmentSubmissionFilesQuery, Result<List<string>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IBunnyUrlSigner _bunnyUrlSigner;
    private readonly BunnyOptions _bunnyOptions;

    public GetAssignmentSubmissionFilesQueryHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, IBunnyUrlSigner bunnyUrlSigner, IOptions<BunnyOptions> bunnyOptions)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _bunnyUrlSigner = bunnyUrlSigner;
        _bunnyOptions = bunnyOptions.Value;
    }

    public async Task<Result<List<string>>> Handle(GetAssignmentSubmissionFilesQuery request, CancellationToken cancellationToken)
    {
        var assignmentResult = await _permissionService.AuthorizeAssignmentAccessAsync(request.AssignmentId);
        if (!assignmentResult.IsSuccess) return Result<List<string>>.Failure(assignmentResult.Error!);

        var submissionFiles = await _unitOfWork.SubmissionFiles.FilterAsync(f => f.AssignmentSubmissionId == request.SubmissionId);

        var signedUrls = submissionFiles
            .Select(f => _bunnyUrlSigner.GenerateSignedUrl(_bunnyOptions.BaseUrl, _bunnyOptions.Token, f.StoragePath, TimeSpan.FromMinutes(2)))
            .ToList();

        return signedUrls;
    }
}
