using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Instructors.Queries.GetMyCourses;

public class GetInstructorCoursesQueryHandler : IRequestHandler<GetInstructorCoursesQuery, Result<List<GetInstructorCoursesDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetInstructorCoursesQueryHandler> _logger;
    private readonly IUserContext _userContext;
    private readonly IBunnyUrlSigner _bunnyUrl;

    public GetInstructorCoursesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetInstructorCoursesQueryHandler> logger, IUserContext userContext, IBunnyUrlSigner bunnyUrl)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _userContext = userContext;
        _bunnyUrl = bunnyUrl;
    }

    public async Task<Result<List<GetInstructorCoursesDto>>> Handle(GetInstructorCoursesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var instructorId = _userContext.GetCurrentUser().Id;
            var courses = await _unitOfWork.Courses.FilterAsync(c => c.InstructorId == instructorId);
            var dto = _mapper.Map<List<GetInstructorCoursesDto>>(courses);
            foreach(var d in dto)
            {
                d.TotalStudents = await _unitOfWork.Enrollments.CountAsync(e => e.CourseId == d.Id);
                d.TotalSections = await _unitOfWork.Sections.CountAsync(s => s.CourseId == d.Id);
                d.ImageUrl = d.ImageUrl == null ? null : _bunnyUrl.GetUrl(d.ImageUrl);
                d.CourseProgress = d.TotalStudents == 0 || d.TotalSections == 0 ? 0 :
                                Math.Round(_unitOfWork.SectionProgress.Query
                                    .Where(s => s.Section.CourseId == d.Id).Count() /
                                (double)(d.TotalSections * d.TotalStudents) * 100, 2);
            }
            return Result<List<GetInstructorCoursesDto>>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while get your courses");
            throw;

        }
    }
}
