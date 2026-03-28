using AutoMapper;
using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Sections.Shared.DTO;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.CreateSection;

public class SectionCreateCommandHandler : IRequestHandler<SectionCreateCommand, Result<SectionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;

    public SectionCreateCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _mapper = mapper;
    }

    public async Task<Result<SectionDto>> Handle(SectionCreateCommand request, CancellationToken cancellationToken)
    {
        var courseResult = await _permissionService.AuthorizeInstructorAccessToCourseAsync(request.CourseId);
        if (!courseResult.IsSuccess) return Result<SectionDto>.Failure(courseResult.Error!);

        var section = _mapper.Map<Section>(request);

        await _unitOfWork.Sections.InsertAsync(section);
        await _unitOfWork.CommitAsync();
        return Result<SectionDto>.Success(_mapper.Map<SectionDto>(section), "Section created successeded");
    }
}
