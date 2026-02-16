using AutoMapper;
using LMS.Application.CurrentUser;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Common.Errors;
using LMS.Domain.Repositories;
using MediatR;
using LMS.Domain.Common.Enums;
using LMS.Domain.Entities.Assignments;
using LMS.Application.DTOs.Assignments;

namespace LMS.Application.Features.Assignments.Commands.CreateAssignment;

public class AssignmentCreateCommandHandler : IRequestHandler<AssignmentCreateCommand, Result<AssignmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IWasabiService _wasabiService;

    public AssignmentCreateCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, IWasabiService wasabiService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
        _wasabiService = wasabiService;
    }

    public async Task<Result<AssignmentDto>> Handle(AssignmentCreateCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);

        if(course == null) 
            return Result<AssignmentDto>.Failure(DomainErrors.Course.NotFound(request.CourseId));

        if(course.InstructorId != userId)
            return Result<AssignmentDto>.Failure(DomainErrors.Common.Forbidden("You do not have permission to create an assignment for this course."));

        var assignment = _mapper.Map<Assignment>(request);
        
        assignment.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Assignments.InsertAsync(assignment);
        await _unitOfWork.CommitAsync();

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

        return Result<AssignmentDto>.Success(dto, "Assignment created successfully.");
    }
}
