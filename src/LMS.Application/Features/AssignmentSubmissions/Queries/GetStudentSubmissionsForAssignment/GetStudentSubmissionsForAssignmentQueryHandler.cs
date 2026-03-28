using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.AssignmentSubmissions.Shared.DTO;
using LMS.Domain.Constants;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.AssignmentSubmissions.Queries.GetStudentSubmissionsForAssignment;

public class GetStudentSubmissionsForAssignmentQueryHandler
    : IRequestHandler<GetStudentSubmissionsForAssignmentQuery, Result<PaginationResult<StudentsAssignmentSubmissionsDto>>>
{
    private readonly IPermissionService _permissionService;
    private readonly IUnitOfWork _unitOfWork;

    public GetStudentSubmissionsForAssignmentQueryHandler(IPermissionService permissionService, IUnitOfWork unitOfWork)
    {
        _permissionService = permissionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginationResult<StudentsAssignmentSubmissionsDto>>> Handle(GetStudentSubmissionsForAssignmentQuery request, CancellationToken cancellationToken)
    {
        var assignmentResult = await _permissionService.AuthorizeInstructorAccessToAssignmentAsync(request.AssignmentId);
        if (!assignmentResult.IsSuccess)
            return Result<PaginationResult<StudentsAssignmentSubmissionsDto>>.Failure(assignmentResult.Error!);

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
