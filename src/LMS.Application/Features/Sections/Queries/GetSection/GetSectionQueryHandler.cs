using AutoMapper;
using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Errors;
using LMS.Domain.Constants;
using LMS.Domain.DTOs.Sections;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Sections.Queries.GetSection;

public class GetSectionQueryHandler : IRequestHandler<GetSectionQuery, Result<CourseSectionsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetSectionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<CourseSectionsDto>> Handle(GetSectionQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();
        var section = await _unitOfWork.Sections.GetAsync(a => a.Id == request.sectionId, [nameof(Section.Course)]);
        if (section == null)
        {
            return Result<CourseSectionsDto>.Failure(DomainErrors.Section.NotFound(request.sectionId));
        }
        if (user.IsInRole(UserRoles.Instructor))
        {
            if (section.Course.InstructorId != user.Id)
            {
                return Result<CourseSectionsDto>.Failure(DomainErrors.Common.Forbidden("You are not the instructor of this course."));
            }
        }
        if (user.IsInRole(UserRoles.Student))
        {
            var isnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(section.CourseId, user.Id);
            if (!isnrolled)
            {
                return Result<CourseSectionsDto>.Failure(DomainErrors.Common.Forbidden("You are not student in this course."));
            }
        }
        var sectiondto = _mapper.Map<CourseSectionsDto>(section);
        return Result<CourseSectionsDto>.Success(sectiondto);

    }
}
