using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.ConfigurationOptions;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Sections.Shared.DTO;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace LMS.Application.Features.Sections.Queries.GetSection;

public class GetSectionQueryHandler : IRequestHandler<GetSectionQuery, Result<CourseSectionsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly BunnyOptions _bunnyOptions;
    private readonly IBunnyUrlSigner _bunnyUrl;

    public GetSectionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext, IOptions<BunnyOptions> bunnyOptions, IBunnyUrlSigner bunnyUrl)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
        _bunnyOptions = bunnyOptions.Value;
        _bunnyUrl = bunnyUrl;
    }

    public async Task<Result<CourseSectionsDto>> Handle(GetSectionQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();
        var section = await _unitOfWork.Sections.GetAsync(a => a.Id == request.sectionId, [nameof(Section.Course),nameof(Section.MaterialFiles)]);
        if (section == null)
        {
            return DomainErrors.Section.NotFound(request.sectionId);
        }
        if (user.IsInRole(UserRoles.Instructor))
        {
            if (section.Course.InstructorId != user.Id)
            {
                return DomainErrors.Common.Forbidden("You are not the instructor of this course.");
            }
        }
        if (user.IsInRole(UserRoles.Student))
        {
            var isnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(section.CourseId, user.Id);
            if (!isnrolled)
            {
                return DomainErrors.Common.Forbidden("You are not student in this course.");
            }
        }

        var sectiondto = _mapper.Map<CourseSectionsDto>(section);

        var materialFiles = section.MaterialFiles
            .OrderBy(f => f.OrderIndex)
            .Select(file => new SectionFileDto
                {
                    FileName = file.FileName,
                    FileSize = file.FileSize,
                    ContentType = file.FileType,
                    OrderIndex = file.OrderIndex,
                    UploadDate = file.UploadDate,
                    FileUrl = _bunnyUrl.GenerateSignedUrl(_bunnyOptions.BaseUrl,
                                                                    _bunnyOptions.Token, file.StoragePath, TimeSpan.FromMinutes(5))
                }).ToList();

        sectiondto.SectionFiles = materialFiles;
        return sectiondto;
    }
}
