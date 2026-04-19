using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Sections.Shared.DTO;
using LMS.Application.Settings;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Options;

namespace LMS.Application.Features.Sections.Queries.GetSection;

public class GetCourseSectionsQueryHandler : IRequestHandler<GetCourseSectionsQuery, Result<List<CourseSectionsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly BunnyOptions _bunnyOptions;
    private readonly IBunnyUrlSigner _bunnyUrl;

    public GetCourseSectionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext, IOptions<BunnyOptions> bunnyOptions, IBunnyUrlSigner bunnyUrl)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
        _bunnyOptions = bunnyOptions.Value;
        _bunnyUrl = bunnyUrl;
    }

    public async Task<Result<List<CourseSectionsDto>>> Handle(GetCourseSectionsQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var course = await _unitOfWork.Courses.GetAsync(c => c.Id == request.CourseId,
            includeProperties: [nameof(Course.Sections)]);

        if(course == null)
            return DomainErrors.Course.NotFound(request.CourseId);

        if (user.IsInRole(UserRoles.Instructor) && course.InstructorId != user.Id)
            return DomainErrors.Course.NotOwned;

        if (user.IsInRole(UserRoles.Student) && !await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, user.Id))
            return DomainErrors.Course.NotEnrolled;

        var result = new List<CourseSectionsDto>();

        var dto = course.Sections.Select(s =>
        {
            var sectionDto = _mapper.Map<CourseSectionsDto>(s);

            sectionDto.SectionFiles = s.MaterialFiles
                .OrderBy(f => f.OrderIndex)
                .Select(file => new SectionFileDto
                {
                    Id = file.Id,
                    FileName = file.FileName,
                    FileSize = file.FileSize,
                    ContentType = file.FileType,
                    OrderIndex = file.OrderIndex,
                    UploadDate = file.UploadDate,
                    FileUrl = _bunnyUrl.GenerateSignedUrl(_bunnyOptions.BaseUrl,
                                            _bunnyOptions.Token, file.StoragePath, TimeSpan.FromMinutes(60))
                }).ToList();

            return sectionDto;
        }).ToList();

        return dto;
    }
}
