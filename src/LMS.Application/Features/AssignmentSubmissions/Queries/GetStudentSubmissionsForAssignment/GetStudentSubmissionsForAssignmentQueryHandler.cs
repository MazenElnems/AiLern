using AutoMapper;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.AssignmentSubmissions.Extensions;
using LMS.Domain.Common.Errors;
using LMS.Domain.DTOs.AssignmentSubmissions;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.AssignmentSubmissions.Queries.GetStudentSubmissionsForAssignment;

public class GetStudentSubmissionsForAssignmentQueryHandler
    : IRequestHandler<GetStudentSubmissionsForAssignmentQuery, Result<PaginationResult<StudentsAssignmentSubmissionsDto>>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStudentSubmissionsForAssignmentQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaginationResult<StudentsAssignmentSubmissionsDto>>> Handle(GetStudentSubmissionsForAssignmentQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        if(!await _unitOfWork.Assignments.AnyAsync(a => a.Id == request.AssignmentId))
            return DomainErrors.Assignment.NotFound(request.AssignmentId);

        if(!await _unitOfWork.Assignments.AnyAsync(a => a.Id == request.AssignmentId && a.Course.InstructorId == userId))
            return DomainErrors.Common.Forbidden("You do not have permission to view submissions for this assignment.");

        var submissionQuery = _unitOfWork.Submissions.Query
            .ApplayAssignmentSubmissionStatusFilter(request.Status)
            .ApplayAssignmentSubmissionSearchFilter(request.SearchString!)
            .Where(s => s.AssignmentId == request.AssignmentId);

        var totalResults = await submissionQuery.CountAsync(cancellationToken);

        var submissions = await submissionQuery
            .Include(s => s.Student)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return PaginationResult<StudentsAssignmentSubmissionsDto>.CreatePaginationResult(
            request.PageNumber,
            request.PageSize,
            totalResults,
            _mapper.Map<List<StudentsAssignmentSubmissionsDto>>(submissions)
        );
    }
}
