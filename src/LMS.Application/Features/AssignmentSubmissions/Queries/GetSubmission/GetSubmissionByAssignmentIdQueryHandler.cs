using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.AssignmentSubmissions.Commands.Submit;
using LMS.Application.Features.AssignmentSubmissions.Shared.DTO;
using LMS.Application.Settings;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Options;

namespace LMS.Application.Features.AssignmentSubmissions.Queries.GetSubmission;

public class GetSubmissionByAssignmentIdQueryHandler : IRequestHandler<GetSubmissionByAssignmentIdQuery, Result<MySubmissionDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IBunnyUrlSigner _bunnyUrlSigner;
    private readonly BunnyOptions _bunnyOptions;

    public GetSubmissionByAssignmentIdQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IUserContext userContext, IBunnyUrlSigner bunnyUrlSigner, IOptions<BunnyOptions> bunnyOptions)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _bunnyUrlSigner = bunnyUrlSigner;
        _bunnyOptions = bunnyOptions.Value;
    }

    public async Task<Result<MySubmissionDto>> Handle(GetSubmissionByAssignmentIdQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var assignment = await _unitOfWork.Assignments.GetAsync(a => a.Id == request.AssignmentId);

        if(assignment == null)
            return DomainErrors.Assignment.NotFound(request.AssignmentId);

        var assignmentSubmission = await _unitOfWork.AssignmentSubmissions
            .GetAsync(a => a.AssignmentId == request.AssignmentId && a.StudentId == user.Id);

        if(assignmentSubmission == null)
            return DomainErrors.AssignmentSubmission.SubmissionNotFound;

        var submissionFiles = await _unitOfWork.SubmissionFiles.FilterAsync(sf => sf.AssignmentSubmissionId == assignmentSubmission.Id);

        var signedUrls = submissionFiles
            .Select(f => new MySubmissionFilesDto
            {
                Id = f.Id,
                FileName = f.FileName,
                FileType = f.FileType,
                FileUrl = _bunnyUrlSigner.GenerateSignedUrl(_bunnyOptions.BaseUrl, _bunnyOptions.Token, f.StoragePath, TimeSpan.FromMinutes(15))
            }).ToList();

        var mySubmission = new MySubmissionDto
        {
            SubmissionId = assignmentSubmission.Id,
            AssignmentId = request.AssignmentId,
            StudentId = user.Id,
            SubmissionDate = assignmentSubmission.SubmissionDate,
            IsLate = assignmentSubmission.IsLate,
            Feedback = assignmentSubmission.Feedback,
            FilesUrls = signedUrls
        };

        return mySubmission;
    }
}
