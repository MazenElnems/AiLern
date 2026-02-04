using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Errors;
using LMS.Domain.DTOs.Assignments;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
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
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<GetAllAssignmentForStudentDto>>(assignmentsWithSubmissions);
    }
}
