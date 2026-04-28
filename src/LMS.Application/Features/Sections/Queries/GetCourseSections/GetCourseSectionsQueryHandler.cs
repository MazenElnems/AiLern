using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Sections.Shared.DTO;
using LMS.Application.Settings;
using LMS.Domain.Constants;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LMS.Application.Features.Sections.Queries.GetSection;

public class GetCourseSectionsQueryHandler : IRequestHandler<GetCourseSectionsQuery, Result<List<CourseSectionsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly BunnyOptions _bunnyOptions;
    private readonly IBunnyUrlSigner _bunnyUrl;

    public GetCourseSectionsQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext, IOptions<BunnyOptions> bunnyOptions, IBunnyUrlSigner bunnyUrl)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _bunnyOptions = bunnyOptions.Value;
        _bunnyUrl = bunnyUrl;
    }

    public async Task<Result<List<CourseSectionsDto>>> Handle(GetCourseSectionsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);

        if(course == null)
            return DomainErrors.Course.NotFound(request.CourseId);

        if (currentUser.IsInRole(UserRoles.Instructor) && course.InstructorId != currentUser.Id)
            return DomainErrors.Course.NotOwned;

        if (currentUser.IsInRole(UserRoles.Student) && !await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, currentUser.Id))
            return DomainErrors.Course.NotEnrolled;

        var sections = await _unitOfWork.Sections.Query
            .Where(s => s.CourseId == request.CourseId)
            .Select(s => new CourseSectionsDto
            {
                Id = s.Id,
                Title = s.Title,
                SectionFiles = s.MaterialFiles.Select(f => new SectionFileDto
                {
                    Id= f.Id,
                    ContentType = f.FileType,
                    FileSize = f.FileSize,
                    FileName = f.FileName,
                    UploadDate = f.UploadDate,
                    OrderIndex = f.OrderIndex,
                    FileUrl = _bunnyUrl.GenerateSignedUrl(_bunnyOptions.BaseUrl, _bunnyOptions.Token,
                                    _bunnyOptions.Token, TimeSpan.FromMinutes(60))
                }).ToList(),
                SectionNumber = s.SectionNumber,
                IsCompleted = s.SectionProgresses.Any(p => p.StudentId == currentUser.Id && p.SectionId == s.Id && p.IsCompleted),
            }).ToListAsync();

        return sections;
    }
}
