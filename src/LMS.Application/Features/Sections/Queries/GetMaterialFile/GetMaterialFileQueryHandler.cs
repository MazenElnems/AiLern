using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Enums;
using LMS.Domain.Constants;
using LMS.Domain.DTOs.MaterialFiles;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Sections.Queries.GetMaterialFile
{
    public class GetMaterialFileQueryHandler : IRequestHandler<GetMaterialFileQuery, Result<MaterialFileMetadataDto>>
    {
        private readonly IUserContext _userContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBunnyUrlSigner _bunnyUrlSigner;
        private readonly IConfiguration _configuration;

        public GetMaterialFileQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork, IBunnyUrlSigner bunnyUrlSigner, IConfiguration configuration)
        {
            _userContext = userContext;
            _unitOfWork = unitOfWork;
            _bunnyUrlSigner = bunnyUrlSigner;
            _configuration = configuration;
        }

        public async Task<Result<MaterialFileMetadataDto>> Handle(GetMaterialFileQuery request, CancellationToken cancellationToken)
        {
            var user = _userContext.GetCurrentUser();

            var section = await _unitOfWork.Sections.GetAsync(sec => sec.Id == request.Id, [nameof(Section.Course), nameof(Section.MaterialFiles)]);

            if (section == null)
                return Result<MaterialFileMetadataDto>.Failure(Domain.Common.Errors.DomainErrors.Section.NotFound(request.Id));

            var course = section.Course;

            if (user.IsInRole(UserRoles.Instructor) && course.InstructorId != user.Id)
                return Result<MaterialFileMetadataDto>.Failure(
                    Domain.Common.Errors.DomainErrors.Common.Forbidden("You are not assigned to this course, so you can’t access its materials."));

            var IsEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(course.Id, user.Id);

            if (user.IsInRole(UserRoles.Student) && !IsEnrolled)
                return Result<MaterialFileMetadataDto>.Failure(
                   Domain.Common.Errors.DomainErrors.Common.Forbidden("You are not enrolled to this course, so you can’t access its materials."));

            var materialFile = section.MaterialFiles.FirstOrDefault(file =>file.UploadStatus == UploadStatus.Completed && file.Id == request.FileId);

            if(materialFile == null)
                return Result<MaterialFileMetadataDto>.Failure(Domain.Common.Errors.DomainErrors.MaterialFile.NotFound(request.FileId));

            var bunnyBaseUrl = _configuration["BunnyCDN:BaseUrl"];
            var bunnyToken = _configuration["BunnyCDN:Token"];


            var source = _bunnyUrlSigner.GenerateSignedUrl(bunnyBaseUrl!, bunnyToken!, materialFile.StoragePath, TimeSpan.FromMinutes(5));

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
}
