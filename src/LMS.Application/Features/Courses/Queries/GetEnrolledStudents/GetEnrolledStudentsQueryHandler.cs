using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Students.Shared.DTO;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Courses.Queries.GetEnrolledStudents;

public class GetEnrolledStudentsQueryHandler : IRequestHandler<GetEnrolledStudentsQuery, Result<PaginationResult<GetEnrolledStudentsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetEnrolledStudentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<PaginationResult<GetEnrolledStudentsDto>>> Handle(GetEnrolledStudentsQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);

        if (course == null)
            return DomainErrors.Course.NotFound(request.CourseId);

        if (course.InstructorId != user.Id)
            return DomainErrors.Course.NotOwned;

        var query = _unitOfWork.Enrollments.Query
            .Where(e => e.CourseId == request.CourseId &&
                   (e.Student.FullName.StartsWith(request.SearchString) || e.Student.Email!.StartsWith(request.SearchString))
            );

        var totalResult = await query.CountAsync(cancellationToken);
        
        var students = await query
            .Skip((request.PageNo - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<GetEnrolledStudentsDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PaginationResult<GetEnrolledStudentsDto>(
            request.PageNo,
            request.PageSize,
            totalResult,
            students
        );
    }
}
