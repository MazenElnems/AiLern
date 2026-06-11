using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Report.Commands.ApproveReport;

public class ApproveMaterialReportCommandHandler : IRequestHandler<ApproveMaterialReportCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApproveMaterialReportCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ApproveMaterialReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _unitOfWork.Reports.GetAsync(r => r.Id == request.ReportId);
        if (report is null)
        {
            return DomainErrors.Report.NotFound(request.ReportId);
        }
        var material = await _unitOfWork.MaterialFiles.GetAsync(m => m.Id == report.MaterialId);
        if (material is null)
        {
            return DomainErrors.MaterialFile.NotFound(report.MaterialId);
        }
        _unitOfWork.MaterialFiles.Delete(report.MaterialFile);
        await _unitOfWork.CommitAsync();
        return Result.Success("Report approved successfully.");
    }
}
