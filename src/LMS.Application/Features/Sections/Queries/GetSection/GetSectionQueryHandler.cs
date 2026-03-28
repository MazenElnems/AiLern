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

public class GetSectionQueryHandler : IRequestHandler<GetSectionQuery, Result<CourseSectionsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;
    private readonly BunnyOptions _bunnyOptions;
    private readonly IBunnyUrlSigner _bunnyUrl;

    public GetSectionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IPermissionService permissionService, IOptions<BunnyOptions> bunnyOptions, IBunnyUrlSigner bunnyUrl)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _permissionService = permissionService;
        _bunnyOptions = bunnyOptions.Value;
        _bunnyUrl = bunnyUrl;
    }

    public async Task<Result<CourseSectionsDto>> Handle(GetSectionQuery request, CancellationToken cancellationToken)
    {
        var sectionResult = await _permissionService.AuthorizeSectionAccessAsync(request.sectionId);
        if (!sectionResult.IsSuccess) return Result<CourseSectionsDto>.Failure(sectionResult.Error!);
        var section = sectionResult.Value!;

        var sectionWithFiles = await _unitOfWork.Sections.GetAsync(s => s.Id == request.sectionId,
            [nameof(Section.MaterialFiles)]);

        var sectiondto = _mapper.Map<CourseSectionsDto>(section);

        var materialFiles = sectionWithFiles!.MaterialFiles
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

        sectiondto.SectionFiles = materialFiles;
        return sectiondto;
    }
}
