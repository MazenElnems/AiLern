using LMS.Application.Common.Results.Generic;
using LMS.Application.ConfigurationOptions;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Enums;
using LMS.Domain.Common.Errors;
using LMS.Domain.Constants;
using LMS.Domain.DTOs.MaterialFiles;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace LMS.Application.Features.Sections.Queries.GetMaterialFile;

public class GetMaterialFileQueryHandler : IRequestHandler<GetMaterialFileQuery, Result<MaterialFileMetadataDto>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBunnyUrlSigner _bunnyUrlSigner;
    private readonly BunnyOptions _bunnyOptions;

    public GetMaterialFileQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork,
        IBunnyUrlSigner bunnyUrlSigner, IOptions<BunnyOptions> bunnyOptions)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _bunnyUrlSigner = bunnyUrlSigner;
        _bunnyOptions = bunnyOptions.Value;
    }

    public async Task<Result<MaterialFileMetadataDto>> Handle(GetMaterialFileQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var section = await _unitOfWork.Sections.GetAsync(sec => sec.Id == request.Id,
            includeProperties: [nameof(Section.Course), nameof(Section.MaterialFiles)]);

        if (section == null)
            return DomainErrors.Section.NotFound(request.Id);

        var course = section.Course;

        if (user.IsInRole(UserRoles.Instructor) && course.InstructorId != user.Id)
            return DomainErrors.Common.Forbidden("You are not assigned to this course, so you can’t access its materials.");

        var IsEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(course.Id, user.Id);

        if (user.IsInRole(UserRoles.Student) && !IsEnrolled)
            return DomainErrors.Common.Forbidden("You are not enrolled to this course, so you can’t access its materials.");

        var materialFile = section.MaterialFiles.FirstOrDefault(file =>file.UploadStatus == UploadStatus.Completed && file.Id == request.FileId);

        if(materialFile == null)
            return DomainErrors.MaterialFile.NotFound(request.FileId);

        var source = _bunnyUrlSigner.GenerateSignedUrl(_bunnyOptions.BaseUrl, _bunnyOptions.Token, materialFile.StoragePath, TimeSpan.FromMinutes(5));

        var result = new MaterialFileMetadataDto
        {
            FileName = materialFile.FileName,
            ContentType = materialFile.FileType,
            FileSize = materialFile.FileSize,
            OrderIndex = materialFile.OrderIndex,
            UploadDate = materialFile.UploadDate,
            FileSource = source
        };

        return Result<MaterialFileMetadataDto>.Success(result, "Material file URL generated successfully.");
    }
}
