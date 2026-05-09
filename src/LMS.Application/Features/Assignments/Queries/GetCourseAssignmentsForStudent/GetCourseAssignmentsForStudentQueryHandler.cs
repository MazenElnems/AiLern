using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Assignments.Shared.DTO;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Assignments.Queries.GetCourseAssignmentsForStudent;

public class GetCourseAssignmentsForStudentQueryHandler : IRequestHandler<GetCourseAssignmentsForStudentQuery, Result<List<GetAllAssignmentForStudentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public GetCourseAssignmentsForStudentQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<Result<List<GetAllAssignmentForStudentDto>>> Handle(GetCourseAssignmentsForStudentQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        if(!await _unitOfWork.Courses.AnyAsync(c => c.Id == request.CourseId))
            return DomainErrors.Course.NotFound(request.CourseId);

        if(!await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, userId))
            return DomainErrors.Course.NotEnrolled;

        var assignmentsWithSubmissions = await _unitOfWork.Assignments.Query
            .Include(a => a.Submissions.Where(s => s.StudentId == userId))
            .Where(a => a.CourseId == request.CourseId && a.IsPublished)
            .OrderByDescending(a=>a.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<GetAllAssignmentForStudentDto>>(assignmentsWithSubmissions);
    }
}
