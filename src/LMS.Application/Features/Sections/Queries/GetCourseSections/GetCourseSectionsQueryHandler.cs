using AutoMapper;
using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.ConfigurationOptions;
using LMS.Application.Features.Sections.Shared.DTO;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace LMS.Application.Features.Sections.Queries.GetSection;

public class GetCourseSectionsQueryHandler : IRequestHandler<GetCourseSectionsQuery, Result<List<CourseSectionsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;
    private readonly BunnyOptions _bunnyOptions;
    private readonly IBunnyUrlSigner _bunnyUrl;

    public GetCourseSectionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IPermissionService permissionService, IOptions<BunnyOptions> bunnyOptions, IBunnyUrlSigner bunnyUrl)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _permissionService = permissionService;
        _bunnyOptions = bunnyOptions.Value;
        _bunnyUrl = bunnyUrl;
    }

    public async Task<Result<List<CourseSectionsDto>>> Handle(GetCourseSectionsQuery request, CancellationToken cancellationToken)
    {
        var courseResult = await _permissionService.AuthorizeCourseAccessAsync(request.CourseId);
        if (!courseResult.IsSuccess) return Result<List<CourseSectionsDto>>.Failure(courseResult.Error!);

        var sections = await _unitOfWork.Sections.FilterAsync(
            s => s.CourseId == request.CourseId,
            includeProperties: [nameof(Section.MaterialFiles)]);

        var dto = sections.Select(s =>
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
                                            _bunnyOptions.Token, file.StoragePath, TimeSpan.FromMinutes(5))
                }).ToList();

            return sectionDto;
        }).ToList();

        return dto;
    }
}
