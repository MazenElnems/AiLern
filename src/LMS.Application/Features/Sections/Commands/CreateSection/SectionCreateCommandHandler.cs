using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Errors;
using LMS.Domain.DTOs.Sections;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.CreateSection;

public class SectionCreateCommandHandler : IRequestHandler<SectionCreateCommand, Result<SectionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;


    public SectionCreateCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<Result<SectionDto>> Handle(SectionCreateCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course == null)
        {
            return Result<SectionDto>.Failure(DomainErrors.Course.NotFound(request.CourseId));
        }
        if (course.InstructorId != userId)
        {
            return Result<SectionDto>.Failure(DomainErrors.Common.Forbidden("You do not have permission to create an assignment for this course."));
        }
        var section = _mapper.Map<Section>(request);

        await _unitOfWork.Sections.InsertAsync(section);
        await _unitOfWork.CommitAsync();
        return Result<SectionDto>.Success(_mapper.Map<SectionDto>(section));
    }
}
