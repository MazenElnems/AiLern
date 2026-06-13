using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.Features.Report.Shared.DTO;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Report.Queries.GetStatistics;

public class GetReportsStatisticsQueryHandler : IRequestHandler<GetReportsStatisticsQuery, Result<ReportsStatisticsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReportsStatisticsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReportsStatisticsDto>> Handle(GetReportsStatisticsQuery request, CancellationToken cancellationToken)
    {
        var reports = await _unitOfWork.Reports.GetAllAsync(includeProperties: [nameof(LMS.Domain.Entities.Courses.Report.MaterialFile)]);
        var totalReports = reports.Count();
        var underReviewReports = reports.Count(r => r.Status == ReportStatus.UnderReview);
        var rejectedReports = reports.Count(r => r.Status == ReportStatus.Rejected);
        var topReportReasons = reports.Where(r => true).GroupBy(s => s.Type).Select(g => new { Reason = g.Key, Count = g.Count() }).ToDictionary(x => x.Reason.ToString(), x => x.Count);
        var topReportForMaterial = reports.Where(r => true).GroupBy(s => s.MaterialFile.FileName).Select(g => new { Name = g.Key, Count = g.Count() }).ToDictionary(x => x.Name.ToString(), x => x.Count);

        var statistics = new ReportsStatisticsDto
        {
            TotalReports = totalReports,
            UnderReviewReports = underReviewReports,
            RejectedReports = rejectedReports,
            TopReportReasons = topReportReasons,
            TopReportForMaterial = topReportForMaterial
            
        };

        return Result<ReportsStatisticsDto>.Success(statistics);
    }
}