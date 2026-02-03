using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Enums;
using LMS.Domain.Common.Errors;
using LMS.Domain.DTOs.Assignments;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
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
        var assignment = await _unitOfWork.Assignments.GetAsync(
            a => a.Id == request.Id,
            [nameof(Assignment.Course)]);

        if (assignment == null)
            return DomainErrors.Assignment.NotFound(request.Id);

        var userId = _userContext.GetCurrentUser().Id;
        if (assignment.Course.InstructorId != userId)
            return DomainErrors.Common.Forbidden("You do not have permission to update this assignment.");

        assignment.Title = request.Title;
        assignment.Instructions = request.Instructions;
        assignment.DueDate = request.DueDate;
        assignment.AllowLateSubmission = request.AllowLateSubmission;
        assignment.IsPublished = request.IsPublished;

        var dto = _mapper.Map<AssignmentDto>(assignment);

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

        await _unitOfWork.CommitAsync();
        return Result<AssignmentDto>.Success(dto, "Assignment updated successfully.");
    }
}
