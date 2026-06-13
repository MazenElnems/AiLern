using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Report.Commands.RejectReport;

public class RejectReportCommandHandler : IRequestHandler<RejectReportCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectReportCommandHandler> _logger;

    public RejectReportCommandHandler(IUnitOfWork unitOfWork, ILogger<RejectReportCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RejectReportCommand request, CancellationToken cancellationToken)
    {

        var report = await _unitOfWork.Reports.GetAsync(r => r.Id == request.ReportId);

        if (report == null)
            return DomainErrors.Report.NotFound(request.ReportId);

        if (report.Status != Domain.Enums.ReportStatus.UnderReview)
            return DomainErrors.Report.ReportAlreadyReviewed;

        var sameReports = await _unitOfWork.Reports.Query.Where(r => r.MaterialId == report.MaterialId 
        && r.Type == report.Type 
        && r.Status == Domain.Enums.ReportStatus.UnderReview).ToListAsync();

        try
        {
            sameReports.ForEach(sr => sr.Status = Domain.Enums.ReportStatus.Rejected);
            await _unitOfWork.CommitAsync();
            
        }catch(Exception ex)
        {
            _logger.LogError(ex, "Error while rejecting report {ReportId}", request.ReportId);
            throw;
        }

        return Result.Success("The report was rejected successfully.");
    }
}
