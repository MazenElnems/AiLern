using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.AssignmentSubmissions.Shared.DTO;
using LMS.Domain.Constants;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.AssignmentSubmissions.Queries.GetStudentSubmissionsForAssignment;

public class GetStudentSubmissionsForAssignmentQueryHandler
    : IRequestHandler<GetStudentSubmissionsForAssignmentQuery, Result<PaginationResult<StudentsAssignmentSubmissionsDto>>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public GetStudentSubmissionsForAssignmentQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginationResult<StudentsAssignmentSubmissionsDto>>> Handle(GetStudentSubmissionsForAssignmentQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        if (!await _unitOfWork.Assignments.AnyAsync(a => a.Id == request.AssignmentId))
            return DomainErrors.Assignment.NotFound(request.AssignmentId);

        if (!await _unitOfWork.Assignments.AnyAsync(a => a.Id == request.AssignmentId && a.Course.InstructorId == userId))
            return DomainErrors.Common.Forbidden("You do not have permission to view submissions for this assignment.");

        var query = _unitOfWork.AssignmentSubmissions.Query
            .AsNoTracking()
            .Where(s => s.AssignmentId == request.AssignmentId);

        if (request.Status != null)
        {
            query = request.Status.ToLower() switch
            {
                AssignmentSubmissionStatus.OnTime => query.Where(submission => !submission.IsLate),
                AssignmentSubmissionStatus.Late => query.Where(submission => submission.IsLate),
                AssignmentSubmissionStatus.All => query,
                _ => query
            };
        }

        var totalResult = await query.CountAsync(cancellationToken);

        var items = await query.Select(s => new StudentsAssignmentSubmissionsDto
        {
            Id = s.Id,
            Email = s.Student.Email!,
            Name = s.Student.FullName,
            StudentId = s.Student.Id,
            IsLate = s.IsLate,
            SubmissionDate = s.SubmissionDate
        }).ToListAsync();

        return PaginationResult<StudentsAssignmentSubmissionsDto>.CreatePaginationResult(
            request.PageNo,
            request.PageSize,
            totalResult,
            items
        );
    }
}
