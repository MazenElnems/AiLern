using AutoMapper;
using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Assignments.Shared.DTO;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.CreateAssignment;

public class AssignmentCreateCommandHandler : IRequestHandler<AssignmentCreateCommand, Result<AssignmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;
    private readonly IWasabiService _wasabiService;
    private readonly IBackgroundJobService _backgroundService;

    public AssignmentCreateCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, IMapper mapper, IWasabiService wasabiService, IBackgroundJobService backgroundService)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _mapper = mapper;
        _wasabiService = wasabiService;
        _backgroundService = backgroundService;
    }

    public async Task<Result<AssignmentDto>> Handle(AssignmentCreateCommand request, CancellationToken cancellationToken)
    {
        var courseResult = await _permissionService.AuthorizeInstructorAccessToCourseAsync(request.CourseId);
        if (!courseResult.IsSuccess) return Result<AssignmentDto>.Failure(courseResult.Error!);
        var course = courseResult.Value!;

        var assignment = _mapper.Map<Assignment>(request);
        assignment.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Assignments.InsertAsync(assignment);
        await _unitOfWork.CommitAsync();

        var dto = _mapper.Map<AssignmentDto>(assignment);
        List<string> keys = new();
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
                keys.Add(key);
            }
        }

        _backgroundService.Schedule<IConfirmUploadedFilesJob>(
            job => job.ExecuteAsync(keys),
            TimeSpan.FromMinutes(2));

        await _unitOfWork.CommitAsync();

        return Result<AssignmentDto>.Success(dto, "Assignment created successfully.");
    }
}
