using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Settings;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Options;

namespace LMS.Application.Features.AssignmentSubmissions.Queries.GetSubmissionFiles;

public class GetAssignmentSubmissionFilesQueryHandler : IRequestHandler<GetAssignmentSubmissionFilesQuery, Result<List<string>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IBunnyUrlSigner _bunnyUrlSigner;
    private readonly BunnyOptions _bunnyOptions;

    public GetAssignmentSubmissionFilesQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext, IBunnyUrlSigner bunnyUrlSigner, IOptions<BunnyOptions> bunnyOptions)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _bunnyUrlSigner = bunnyUrlSigner;
        _bunnyOptions = bunnyOptions.Value;
    }

    public async Task<Result<List<string>>> Handle(GetAssignmentSubmissionFilesQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var assignment = await _unitOfWork.Assignments.GetAsync(a => a.Id == request.AssignmentId,
            includeProperties: [nameof(Assignment.Course)]);

        if(assignment == null)
            return DomainErrors.Assignment.NotFound(request.AssignmentId);

        var course = assignment.Course;

        if (user.IsInRole(UserRoles.Instructor) && course.InstructorId != user.Id)
            return DomainErrors.Assignment.NotOwned;

        if(user.IsInRole(UserRoles.Student) && !await _unitOfWork.Enrollments.IsEnrolledAsync(course.Id, user.Id))
            return DomainErrors.Course.NotEnrolled;

        var submissionFiles = await _unitOfWork.SubmissionFiles.FilterAsync(f => f.AssignmentSubmissionId == request.SubmissionId);

        var signedUrls = submissionFiles
            .Select(f => _bunnyUrlSigner.GenerateSignedUrl(_bunnyOptions.BaseUrl, _bunnyOptions.Token, f.StoragePath, TimeSpan.FromMinutes(2)))
            .ToList();

        return signedUrls;
    }
}
