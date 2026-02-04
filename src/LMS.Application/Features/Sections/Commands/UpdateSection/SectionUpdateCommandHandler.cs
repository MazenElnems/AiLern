using AutoMapper;
using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Common.Errors;
using LMS.Domain.Entities;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.UpdateSection;

public class SectionUpdateCommandHandler : IRequestHandler<SectionUpdateCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public SectionUpdateCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<Result> Handle(SectionUpdateCommand request, CancellationToken cancellationToken)
    {
        var section = await _unitOfWork.Sections.GetAsync(a => a.Id == request.Id,
            [nameof(Section.Course), nameof(Section.MaterialFiles)]);
        if (section == null)
        {
            return Result.Failure(DomainErrors.Section.NotFound(request.Id)); 
        }
        var userId = _userContext.GetCurrentUser().Id;
        if (section.Course.InstructorId != userId)
            return Result.Failure(DomainErrors.Common.Forbidden("You do not have permission to update this section."));
        section.Title = request.Title;
        section.SectionNumber = request.SectionNumber;

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
