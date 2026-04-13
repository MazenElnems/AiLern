using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Assignments.Shared.DTO;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.UpdateAssignment;

public class AssignmentUpdateCommandHandler : IRequestHandler<AssignmentUpdateCommand, Result<AssignmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IWasabiService _wasabiService;

    public AssignmentUpdateCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, IWasabiService wasabiService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
        _wasabiService = wasabiService;
    }

    public async Task<Result<AssignmentDto>> Handle(AssignmentUpdateCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var assignment = await _unitOfWork.Assignments.GetAsync(a => a.Id == request.Id,
            includeProperties: [nameof(Assignment.Course), nameof(Assignment.Submissions)]);

        if (assignment == null)
            return DomainErrors.Assignment.NotFound(request.Id);

        if (assignment.Course.InstructorId != userId)
            return DomainErrors.Course.NotOwned;

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
            foreach (var submission in assignment.Submissions)
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
                var key = $"courses/{assignment.Course.Name}/assignments/{assignment.Id}/{Guid.NewGuid()}_{file.FileName}";
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
