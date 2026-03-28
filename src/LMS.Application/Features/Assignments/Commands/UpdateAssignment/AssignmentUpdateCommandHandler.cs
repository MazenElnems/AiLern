using AutoMapper;
using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Assignments.Shared.DTO;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.UpdateAssignment;

public class AssignmentUpdateCommandHandler : IRequestHandler<AssignmentUpdateCommand, Result<AssignmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;
    private readonly IWasabiService _wasabiService;

    public AssignmentUpdateCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, IMapper mapper, IWasabiService wasabiService)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _mapper = mapper;
        _wasabiService = wasabiService;
    }

    public async Task<Result<AssignmentDto>> Handle(AssignmentUpdateCommand request, CancellationToken cancellationToken)
    {
        var assignmentResult = await _permissionService.AuthorizeInstructorAccessToAssignmentAsync(request.Id);
        if (!assignmentResult.IsSuccess) return Result<AssignmentDto>.Failure(assignmentResult.Error!);
        var assignment = assignmentResult.Value!;
        var course = assignment.Course;

        if (assignment.DueDate > request.DueDate)
            return DomainErrors.Assignment.InValidDueDate;

        var previousDueDate = assignment.DueDate;

        assignment.Title = request.Title;
        assignment.Instructions = request.Instructions;
        assignment.DueDate = request.DueDate;
        assignment.AllowLateSubmission = request.AllowLateSubmission;
        assignment.IsPublished = request.IsPublished;

        if (assignment.IsPublished && request.DueDate > previousDueDate)
        {
            var submissions = await _unitOfWork.AssignmentSubmissions
                .FilterAsync(s => s.AssignmentId == request.Id);
            foreach (var submission in submissions)
            {
                if (submission.IsLate && submission.SubmissionDate <= request.DueDate)
                    submission.IsLate = false;
            }
        }

        var dto = _mapper.Map<AssignmentDto>(assignment);

        if (request.UploadedFileMetaData is not null)
        {
            foreach (var file in request.UploadedFileMetaData)
            {
                var key = $"courses/{course.Name}/assignments/{assignment.Id}/{Guid.NewGuid()}_{file.FileName}";
                var url = await _wasabiService.GeneratePresignedUploadUrlAsync(key, file.ContentType, 2);
                dto.PresingedFileUrls.Add(url);

                assignment.Files.Add(new AssignmentFile
                {
                    AssignmentId = assignment.Id,
                    FileName = file.FileName,
                    FileType = file.ContentType,
                    StoragePath = key,
                    UploadStatus = UploadStatus.Pending,
                });
            }
        }

        await _unitOfWork.CommitAsync();
        return Result<AssignmentDto>.Success(dto, "Assignment updated successfully.");
    }
}
