using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LMS.Application.Features.Report.Commands.CreateReport;

public class CreateMaterialReportCommandHandler : IRequestHandler<CreateMaterialReportCommand, Result>
{
    private readonly IUserContext _user;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMaterialReportCommandHandler(IUserContext user, IUnitOfWork unitOfWork)
    {
        _user = user;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateMaterialReportCommand request, CancellationToken cancellationToken)
    {
        var userId = _user.GetCurrentUser().Id;
        var section = await _unitOfWork.Sections.GetAsync(s =>s.Id == request.SectionId);
        var material = await _unitOfWork.MaterialFiles.GetAsync(m => m.Id == request.MaterialId);
        if (section is null)
        {
            return DomainErrors.Section.NotFound(request.SectionId);
        }
        if (material is null)
        {
            return DomainErrors.MaterialFile.NotFound(request.MaterialId);
        }
        var course = await _unitOfWork.Courses.GetAsync(c => c.Id == section.CourseId);
        var enrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(course!.Id,userId);
        if (!enrolled)
        {
            return DomainErrors.Course.NotEnrolled;
        }
        var alreadreported = await _unitOfWork.Reports.AnyAsync(r => r.MaterialId == request.MaterialId && r.StudentId == userId);
        if (alreadreported)
        {
            return DomainErrors.Report.AlreadyReported(request.MaterialId, userId);
        }
        var report = new Domain.Entities.Courses.Report
        {
            Comment = request.Reason,
            MaterialId = request.MaterialId,
            StudentId = userId,
            Type = request.ReportType,
            SubmittedAt = DateTime.UtcNow,
            Status = ReportStatus.UnderReview,
        
        };
        await _unitOfWork.Reports.InsertAsync(report);
        await _unitOfWork.CommitAsync();
        return Result.Success("Report submitted successfully");

    }
}
