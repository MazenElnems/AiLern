using AutoMapper;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Courses.Queries.GetAllCourses;

public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, Result<PaginationResult<GetAllCoursesDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCoursesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaginationResult<GetAllCoursesDto>>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Courses.Query
            .AsNoTracking();

        var totalResult = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.PageSize * (request.PageNo - 1))
            .Take(request.PageSize)
            .Select(c => new GetAllCoursesDto
            {
                Id = c.Id,
                Code = c.Code,
                CreatedAt = c.CreatedAt,
                Name = c.Name,
                InstructorName = c.Instructor.FullName,
                InstructorId = c.InstructorId,
                ImageUrl = c.ImageStoragePath == null ? null : $"https://ailern-content.b-cdn.net/{c.ImageStoragePath}"
            }).ToListAsync();

        return new PaginationResult<GetAllCoursesDto>(
            request.PageNo,
            request.PageSize,
            totalResult,
            items
        );
    }
}
