using AutoMapper;
using LMS.Application.Common.Results.Generic;
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

    public GetInstructorCoursesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetInstructorCoursesQueryHandler> logger, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<Result<List<GetInstructorCoursesDto>>> Handle(GetInstructorCoursesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var instructorId = _userContext.GetCurrentUser().Id;
            var courses = await _unitOfWork.Courses.FilterAsync(c => c.InstructorId == instructorId);
            var dto = _mapper.Map<List<GetInstructorCoursesDto>>(courses);
            return Result<List<GetInstructorCoursesDto>>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while get your courses");
            throw;

        }
    }
}
