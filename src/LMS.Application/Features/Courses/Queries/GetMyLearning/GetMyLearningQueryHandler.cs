using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Courses.Queries.GetMyLearning;

public class GetMyLearningQueryHandler
    : IRequestHandler<GetMyLearningQuery, Result<PaginationResult<GetMyLearningDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public GetMyLearningQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<PaginationResult<GetMyLearningDto>>> Handle(GetMyLearningQuery request, CancellationToken cancellationToken)
    {
        var studentId = _userContext.GetCurrentUser().Id;

        var query = _unitOfWork.CourseProgress.Query
            .Where(p => p.StudentId == studentId);

        var totalResult = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.UpdatedAt)
            .Select(p => new GetMyLearningDto
            {
                CourseId = p.CourseId,
                LastLearningItemId = p.LastOpenedFileId,
                LastPageNumber = p.LastPageNumber,
                LastWatchedTime = p.LastWatchedTime,
                Type = p.Type,
                Name = p.Course.Name
            })
            .Skip((request.PageNo - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginationResult<GetMyLearningDto>(
            request.PageNo,
            request.PageSize,
            totalResult,
            items
        );
    }
}
