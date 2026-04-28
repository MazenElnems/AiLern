using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Enums;
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

    public async Task<Result<PaginationResult<GetMyLearningDto>>> Handle(
        GetMyLearningQuery request,
        CancellationToken cancellationToken)
    {
        //var userId = _userContext.GetCurrentUser().Id;

        //var ordered = _unitOfWork.Enrollments.Query
        //    .AsNoTracking()
        //    .Where(e => e.StudentId == userId)
        //    .Select(e => new
        //    {
        //        e.CourseId,
        //        Name = e.Course.Name,
        //        e.EnrolledAt,
        //        Progress = e.Course.Progresses.Where(p => p.StudentId == userId).FirstOrDefault()
        //    })
        //    .OrderByDescending(x => x.Progress != null ? x.Progress.UpdatedAt : x.EnrolledAt);

        //var totalResult = await ordered.CountAsync(cancellationToken);

        //var items = await ordered
        //    .Skip((request.PageNo - 1) * request.PageSize)
        //    .Take(request.PageSize)
        //    .Select(x => new GetMyLearningDto
        //    {
        //        CourseId = x.CourseId,
        //        Name = x.Name,
        //        Percent = x.Progress != null ? x.Progress.Percent : 0,
        //        LastLearningItemId = x.Progress != null ? x.Progress.LastLearningItemId : null,
        //        LastPageNumber = x.Progress != null ? x.Progress.LastPageNumber : null,
        //        LastWatchedTime = x.Progress != null ? x.Progress.LastWatchedTime : null,
        //        Type = x.Progress != null ? x.Progress.Type : LearningType.None
        //    })
        //    .ToListAsync(cancellationToken);

        return new PaginationResult<GetMyLearningDto>(
            request.PageNo,
            request.PageSize,
            0,
            new List<GetMyLearningDto>());
    }
}
