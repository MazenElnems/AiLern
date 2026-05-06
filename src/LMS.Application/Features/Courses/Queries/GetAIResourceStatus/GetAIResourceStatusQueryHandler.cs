using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Courses.Shared.DTO;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Courses.Queries.GetAIResourceStatus;

public class GetAIResourceStatusQueryHandler : IRequestHandler<GetAIResourceStatusQuery, Result<List<AIStatusDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public GetAIResourceStatusQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<List<AIStatusDto>>> Handle(GetAIResourceStatusQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        if(!await _unitOfWork.Courses.AnyAsync(c => c.Id == request.CourseId))
            return DomainErrors.Course.NotFound(request.CourseId);

        if (!await _unitOfWork.Courses.AnyAsync(c => c.Id == request.CourseId && c.InstructorId == userId))
            return DomainErrors.Course.NotEnrolled;

        var aiResourceStatuses = _unitOfWork.AIResources.Query
            .Where(a => a.CourseId == request.CourseId)
            .Select(a => new AIStatusDto
            {
                Id = a.Id,
                AIStatus = a.AIStatus
            }) .ToList();

        return aiResourceStatuses;
    }
}
