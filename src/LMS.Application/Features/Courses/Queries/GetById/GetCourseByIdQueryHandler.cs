using AutoMapper;
using LMS.Application.Common.Results.Generic;
using MediatR;
using LMS.Domain.Errors;
using LMS.Application.Features.Courses.Shared.DTO;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.Contracts.Services;

namespace LMS.Application.Features.Courses.Queries.GetById;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, Result<GetCourseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBunnyUrlSigner _bunnyUrl;

    public GetCourseByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IBunnyUrlSigner bunnyUrl)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _bunnyUrl = bunnyUrl;
    }

    public async Task<Result<GetCourseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdWithDetailsAsync(request.Id)
            ;
        if (course == null)
            return Result<GetCourseDto>.Failure(DomainErrors.Course.NotFound(request.Id));
        var dto = _mapper.Map<GetCourseDto>(course);
        dto.ImageUrl = dto.ImageUrl == null ? null : _bunnyUrl.GetUrl(dto.ImageUrl);
        return Result<GetCourseDto>.Success(dto);
    }
}
