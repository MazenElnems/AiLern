using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.UpdateSection;

public class SectionUpdateCommandHandler : IRequestHandler<SectionUpdateCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;

    public SectionUpdateCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
    }

    public async Task<Result> Handle(SectionUpdateCommand request, CancellationToken cancellationToken)
    {
        var sectionResult = await _permissionService.AuthorizeInstructorAccessToSectionAsync(request.Id);
        if (!sectionResult.IsSuccess) return Result.Failure(sectionResult.Error!);
        var section = sectionResult.Value!;

        section.Title = request.Title;
        section.SectionNumber = request.SectionNumber;

        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
