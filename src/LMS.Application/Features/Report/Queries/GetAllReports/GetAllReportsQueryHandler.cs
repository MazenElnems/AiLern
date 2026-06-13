using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.Features.Report.Shared.DTO;
using LMS.Application.Features.Users.Shared.DTO;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Report.Queries.GetAllReports;

public class GetAllReportsQueryHandler : IRequestHandler<GetAllReportsQuery, Result<PaginationResult<GetAllReportsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllReportsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginationResult<GetAllReportsDto>>> Handle(GetAllReportsQuery request, CancellationToken cancellationToken)
    {
        bool invalidType = request.Type.HasValue && !Enum.IsDefined(typeof(ReportType), request.Type.Value);

        if(invalidType)
            return new PaginationResult<GetAllReportsDto>(
                request.PageNo,
                request.PageSize,
                0,
                []
            );

        var query = _unitOfWork.Reports.Query
            .Where(r =>
                r.Status == ReportStatus.UnderReview &&
                (!request.Type.HasValue || r.Type == request.Type.Value));

        var totalResult = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.SubmittedAt)
            .Skip((request.PageNo - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new GetAllReportsDto
            {
                ReportId = r.Id,
                Material = r.MaterialFile.FileName,
                Status = r.Status.ToString(),
                Reason = r.Type.ToString(),
                Date = r.SubmittedAt,
                Reporter = r.Student.FullName,
                Comment = r.Comment
            })
            .ToListAsync(cancellationToken);


        return new PaginationResult<GetAllReportsDto>(request.PageNo, request.PageSize, totalResult, items);


    }
}
