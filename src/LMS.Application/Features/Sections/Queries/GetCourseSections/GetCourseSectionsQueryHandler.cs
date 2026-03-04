using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.ConfigurationOptions;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Sections.DTO;
using LMS.Application.Features.Sections.Shared.DTO;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
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

        var sections = await _unitOfWork.Sections.FilterAsync(a => a.CourseId == request.CourseId,
            includeProperties: [nameof(Section.MaterialFiles),nameof(Section.Course)]);

        var course = sections.FirstOrDefault()==null?null:sections.FirstOrDefault().Course;

        if (course == null)
        {
            return DomainErrors.Course.NotFound(request.CourseId);
        }


        if (user.IsInRole(UserRoles.Instructor))
        {
            if (course.InstructorId != user.Id)
            {
                return DomainErrors.Common.Forbidden("You are not the instructor of this course.");
            }
        }
        if (user.IsInRole(UserRoles.Student))
        {
            var isnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, user.Id);
            if (!isnrolled)
            {
                return DomainErrors.Common.Forbidden("You are not student in this course.");
            }
        }

        var result = new List<CourseSectionsDto>();

        foreach (var section in sections)
        {
            result.Add(_mapper.Map<CourseSectionsDto>(section));

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
                                                            _bunnyOptions.Token,file.StoragePath, TimeSpan.FromMinutes(5))
                                }).ToList();

            result.Last().SectionFiles = materialFiles;

        }

        return result;
    }
}
