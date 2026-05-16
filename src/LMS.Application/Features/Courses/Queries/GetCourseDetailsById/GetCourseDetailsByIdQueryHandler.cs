using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.Features.Courses.Shared.DTO;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace LMS.Application.Features.Courses.Queries.GetCourseDetailsById;

public class GetCourseDetailsByIdQueryHandler : IRequestHandler<GetCourseDetailsByIdQuery, Result<GetCourseDetailsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBunnyUrlSigner _bunnyUrl;
    public GetCourseDetailsByIdQueryHandler(IUnitOfWork unitOfWork, IBunnyUrlSigner bunnyUrl)
    {
        _unitOfWork = unitOfWork;
        _bunnyUrl = bunnyUrl;
    }

    public async Task<Result<GetCourseDetailsDto>> Handle(GetCourseDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetAsync(c => c.Id == request.courseId, includeProperties: [nameof(Course.Instructor)]);
        if (course == null)
            return DomainErrors.Course.NotFound(request.courseId);

        var TotalEnrollments = await _unitOfWork.Enrollments.Query
            .AsNoTracking().Where(e => e.CourseId == request.courseId).CountAsync();

        var AiResources = await _unitOfWork.AIResources.Query
            .AsNoTracking().Where(ar => ar.CourseId == request.courseId)
            .GroupBy(x => 1)
            .Select(g => new { ResourcesNumber = g.Count(), ResourcesSize = g.Sum(ar => ar.FileSize) })
            .FirstOrDefaultAsync();

        if (AiResources == null)
            AiResources = new { ResourcesNumber = 0, ResourcesSize = 0d };

        var sections = await _unitOfWork.Sections.FilterAsync(sec => sec.CourseId == request.courseId,
            includeProperties: [nameof(Section.MaterialFiles)]);

        var Masterials = sections.SelectMany(sec => sec.MaterialFiles).GroupBy(m => 1)
            .Select(g => new { MaterialNumber = g.Count(), MaterialSize = g.Sum(m => m.FileSize) })
            .FirstOrDefault();

        if (Masterials == null)
            Masterials = new { MaterialNumber = 0, MaterialSize = 0L };

        var dto = new GetCourseDetailsDto
        {
            Id = course.Id,
            imagePath = course.ImageStoragePath == null ? null : _bunnyUrl.GetUrl(course.ImageStoragePath),
            CourseName = course.Name,
            CourseCode = course.Code,
            CourseDescription = course.Description ?? null,
            InstructorId = course.InstructorId,
            InstructorName = course.Instructor.FullName,
            InstructorEmail = course.Instructor.Email!,
            InstructorImage = course.Instructor.ImageStoragePath == null ? null : _bunnyUrl.GetUrl(course.Instructor.ImageStoragePath),
            TotalAiResourcesNumber = AiResources!.ResourcesNumber,
            TotalAiResourcesSize = Convert.ToInt64(AiResources.ResourcesSize),
            TotalEnrollments = TotalEnrollments,
            TotalMaterialNumber = Masterials!.MaterialNumber,
            TotalMaterialSize = Masterials.MaterialSize
        };

        return dto;
    }
}
