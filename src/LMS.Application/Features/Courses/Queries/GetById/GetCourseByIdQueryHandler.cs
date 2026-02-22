using AutoMapper;
using LMS.Domain.Repositories;
using LMS.Application.Common.Results.Generic;
using MediatR;
using LMS.Domain.Errors;
using LMS.Application.Features.Courses.DTO;

namespace LMS.Application.Features.Courses.Queries.GetById;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, Result<GetCourseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCourseByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<GetCourseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdWithDetailsAsync(request.Id)
            ;
        if (course == null)
            return Result<GetCourseDto>.Failure(DomainErrors.Course.NotFound(request.Id));
        var dto = _mapper.Map<GetCourseDto>(course);
        return Result<GetCourseDto>.Success(dto);
    }
}
