using AutoMapper;
using LMS.Domain.Repositories;
using LMS.Domain.DTOs.Courses;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using MediatR;

namespace LMS.Application.Queries.Courses.GetByIdQueries;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, GetCourseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCourseByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GetCourseDto> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdWithDetailsAsync(request.Id)
            ?? throw new ResourceNotFoundException(nameof(Course), request.Id.ToString());
        var dto = _mapper.Map<GetCourseDto>(course);
        return dto;
    }
}
