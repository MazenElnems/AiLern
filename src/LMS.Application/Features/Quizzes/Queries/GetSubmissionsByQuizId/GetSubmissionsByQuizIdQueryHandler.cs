using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Quizzes.Queries.GetSubmissionsByQuizId;

public class GetSubmissionsByQuizIdQueryHandler : IRequestHandler<GetSubmissionsByQuizIdQuery, Result<PaginationResult<GetSubmissionsByQuizIdDto>>>
{
    private readonly IPermissionService _permissionService;
    private readonly IUnitOfWork _unitOfWork;

    public GetSubmissionsByQuizIdQueryHandler(IPermissionService permissionService, IUnitOfWork unitOfWork)
    {
        _permissionService = permissionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginationResult<GetSubmissionsByQuizIdDto>>> Handle(GetSubmissionsByQuizIdQuery request, CancellationToken cancellationToken)
    {
        var quizResult = await _permissionService.AuthorizeInstructorAccessToQuizAsync(request.QuizId);
        if (!quizResult.IsSuccess) return Result<PaginationResult<GetSubmissionsByQuizIdDto>>.Failure(quizResult.Error!);

        var query = _unitOfWork.Attempts.Query
            .AsNoTracking()
            .Where(a => a.QuizId == request.QuizId && a.Status == request.Status);

        var totalResult = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.PageSize * (request.PageNo - 1))
            .Take(request.PageSize)
            .Select(a => new GetSubmissionsByQuizIdDto
            {
                Id = a.Id,
                AttemptNumber = a.AttemptNumber,
                Score = a.AttemptAnswers.Sum(aa => aa.Mark),
                StartAt = a.StartAt,
                Status = a.Status,
                StudentId = a.StudentId,
                StudentName = a.Student.FullName,
                SubmittedAt = a.SubmittedAt,
                TimeSpent = a.TimeSpent
            }).ToListAsync();

        return new PaginationResult<GetSubmissionsByQuizIdDto>(
            request.PageNo,
            request.PageSize,
            totalResult,
            items
        );
    }
}
