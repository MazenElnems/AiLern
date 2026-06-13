using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.Features.Report.Shared.DTO;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Report.Queries.GetReportById;

public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, Result<GetReportByIdDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBunnyUrlSigner _bunnyUrl;

    public GetReportByIdQueryHandler(IUnitOfWork unitOfWork, IBunnyUrlSigner bunnyUrl)
    {
        _unitOfWork = unitOfWork;
        _bunnyUrl = bunnyUrl;
    }

    public async Task<Result<GetReportByIdDto>> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        var report = await _unitOfWork.Reports.GetAsync(r => r.Id == request.ReportId,
            includeProperties: [nameof(Domain.Entities.Courses.Report.MaterialFile)]);

        if (report == null)
            return DomainErrors.Report.NotFound(request.ReportId);

        if (report.MaterialFile == null)
            return DomainErrors.MaterialFile.NotFound(report.MaterialId);

        var reporter = await _unitOfWork.Users.Query.AsNoTracking()
            .Where(u => u.Id == report.StudentId)
            .Select(u=> new {Id = u.Id, Name = u.FullName, Email = u.Email})
            .FirstOrDefaultAsync();

        if (reporter == null)
            return DomainErrors.User.NotFound(report.StudentId.ToString());

        var section = await _unitOfWork.Sections.GetAsync(s => s.Id == report.MaterialFile.SectionId,
            includeProperties: [nameof(Section.Course)]);

        if (section == null)
            return DomainErrors.Section.NotFound(report.MaterialFile.SectionId);

        var Instructor = await _unitOfWork.Users.Query.AsNoTracking()
            .Where(u => u.Id == section.Course.InstructorId).Select(u => new { Id = u.Id, Name = u.FullName, Email = u.Email })
            .FirstOrDefaultAsync();

        if (Instructor == null)
            return DomainErrors.User.NotFound(section.Course.InstructorId.ToString());

        var previewMaterialUrl = _bunnyUrl.GenerateSignedUrl(report.MaterialFile.StoragePath, TimeSpan.FromMinutes(60));

        var result = new GetReportByIdDto
        {
            ReportId = report.Id,
            ReportType = report.Type,
            SubmittedAt = report.SubmittedAt,
            ReportComment = report.Comment,
            ReportStatus = report.Status,

            ReporterId = reporter.Id,
            ReporterName = reporter.Name,
            ReporterEmail = reporter.Email!,

            MaterialId = report.MaterialFile.Id,
            MaterialName = report.MaterialFile.FileName,
            MaterialType = report.MaterialFile.FileType,

            CourseId = section.CourseId,
            CourseName = section.Course.Name,

            InstructorId = Instructor.Id,
            InstructorName = Instructor.Name,
            InstructorEmail = Instructor.Email!,

            PreviewMaterialUrl = previewMaterialUrl

        };

        return result;
    }
}
