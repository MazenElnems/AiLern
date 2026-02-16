using AutoMapper;
using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Errors;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.UpdateSection;

public class SectionUpdateCommandHandler : IRequestHandler<SectionUpdateCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public SectionUpdateCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(SectionUpdateCommand request, CancellationToken cancellationToken)
    {
        var section = await _unitOfWork.Sections.GetAsync(a => a.Id == request.Id,
            includeProperties: [nameof(Section.Course)]);
        if (section == null)
        {
            return DomainErrors.Section.NotFound(request.Id); 
        }
        var userId = _userContext.GetCurrentUser().Id;
        if (section.Course.InstructorId != userId)
            return DomainErrors.Common.Forbidden("You do not have permission to update this section.");
        section.Title = request.Title;
        section.SectionNumber = request.SectionNumber;

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
