using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Students.Queries.GetMyCourses;

public class GetStudentCoursesQueryHandler : IRequestHandler<GetStudentCoursesQuery, Result<PaginationResult<GetStudentCoursesDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IBunnyUrlSigner _bunnyUrl;

    public GetStudentCoursesQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext, IBunnyUrlSigner bunnyUrl)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _bunnyUrl = bunnyUrl;
    }

    public async Task<Result<PaginationResult<GetStudentCoursesDto>>> Handle(GetStudentCoursesQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var query = _unitOfWork.Courses.Query
            .AsNoTracking()
            .Where(c => c.Enrollments.Any(e => e.StudentId == userId));

        var totalResult = await query.CountAsync(cancellationToken);

        var items = await query.Select(c => new GetStudentCoursesDto
        {
            Id = c.Id,
            Code = c.Code,
            Name = c.Name,
            Description = c.Description,
            InstructorId = c.InstructorId,
            InstructorName = c.Instructor.FullName,
            Progress = c.Sections.Count() == 0
                ? 0
                : (int) (c.Sections.Count(s => s.SectionProgresses.Any(p => p.IsCompleted && p.StudentId == userId)) * 100M
                    /
                  c.Sections.Count()) ,
            ImageUrl = c.ImageStoragePath == null ? null : _bunnyUrl.GetUrl(c.ImageStoragePath)

        }).ToListAsync();

        return new PaginationResult<GetStudentCoursesDto>(
            request.PageNo,
            request.PageSize,
            totalResult,
            items
        );
    }
}
