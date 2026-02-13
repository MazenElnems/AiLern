using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Errors;
using LMS.Domain.Constants;
using LMS.Domain.DTOs.Sections;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Sections.Queries.GetSection;

public class GetCourseSectionsQueryHandler : IRequestHandler<GetCourseSectionsQuery, Result<List<CourseSectionsDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetCourseSectionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<List<CourseSectionsDto>>> Handle(GetCourseSectionsQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();
        var course = await _unitOfWork.Courses.GetAsync(a => a.Id == request.CourseId, [nameof(Course.Sections)]);
        if (course == null)
        {
            return Result<List<CourseSectionsDto>>.Failure(DomainErrors.Course.NotFound(request.CourseId));
        }


        if (user.IsInRole(UserRoles.Instructor))
        {
            if (course.InstructorId != user.Id)
            {
                return Result<List<CourseSectionsDto>>.Failure(DomainErrors.Common.Forbidden("You are not the instructor of this course."));
            }
        }
        if (user.IsInRole(UserRoles.Student))
        {
            var isnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, user.Id);
            if (!isnrolled)
            {
                return Result<List<CourseSectionsDto>>.Failure(DomainErrors.Common.Forbidden("You are not student in this course."));
            }
        }



        var result = new List<CourseSectionsDto>();

        foreach (var section in course.Sections)
        {
        result.Add(_mapper.Map<CourseSectionsDto>(section));

        }

        return Result<List<CourseSectionsDto>>.Success(result);

    }

}

