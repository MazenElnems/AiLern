using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Constants;
using LMS.Domain.DTOs.MaterialFiles;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Sections.Queries.GetSectionFiles
{
    public class GetSectionFilesQueryHandler : IRequestHandler<GetSectionFilesQuery, Result<List<MaterialFileMetadataDto>>>
    {
        private readonly IUserContext _userContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetSectionFilesQueryHandler> _logger;

        public GetSectionFilesQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork, ILogger<GetSectionFilesQueryHandler> logger)
        {
            _userContext = userContext;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<List<MaterialFileMetadataDto>>> Handle(GetSectionFilesQuery request, CancellationToken cancellationToken)
        {
            try 
            {
                var user = _userContext.GetCurrentUser();

                var section = await _unitOfWork.Sections.GetAsync(sec => sec.Id == request.sectionId, [nameof(Section.Course), nameof(Section.MaterialFiles)]);
                if (section == null)
                    return Result<List<MaterialFileMetadataDto>>.Failure(Domain.Common.Errors.DomainErrors.Section.NotFound(request.sectionId));
                var course = section.Course;

                if (user.IsInRole(UserRoles.Instructor) && course.InstructorId != user.Id)
                    return Result<List<MaterialFileMetadataDto>>.Failure(
                        Domain.Common.Errors.DomainErrors.Common.Forbidden("You are not assigned to this course, so you can’t access its materials."));

                var IsEnrolled =await _unitOfWork.Enrollments.IsEnrolledAsync(course.Id,user.Id);

                if(user.IsInRole(UserRoles.Student) && !IsEnrolled)
                    return Result<List<MaterialFileMetadataDto>>.Failure(
                       Domain.Common.Errors.DomainErrors.Common.Forbidden("You are not enrolled to this course, so you can’t access its materials."));

                var result = section.MaterialFiles.Select(file => new MaterialFileMetadataDto
                {
                    FileName = file.FileName,
                    FileSize = file.FileSize,
                    ContentType = file.FileType,
                    OrderIndex = file.OrderIndex,
                    UploadDate = file.UploadDate

                }).OrderBy(f=>f.OrderIndex).ToList();

                return Result<List<MaterialFileMetadataDto>>.Success(result, "Material files retrieved successfully"); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving material files.");
                throw;
            }
        }
    }
}
