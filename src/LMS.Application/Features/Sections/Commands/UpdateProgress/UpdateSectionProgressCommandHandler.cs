using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.UpdateProgress;

internal class UpdateSectionProgressCommandHandler : IRequestHandler<UpdateSectionProgressCommand, Result>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSectionProgressCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateSectionProgressCommand request, CancellationToken cancellationToken)
    {
        var studentId = _userContext.GetCurrentUser().Id;

        var section = await _unitOfWork.Sections.GetByIdAsync(request.SectionId);

        if(section == null)
            return DomainErrors.Section.NotFound(request.SectionId);

        if (!await _unitOfWork.Enrollments.IsEnrolledAsync(section.CourseId, studentId))
            return DomainErrors.Course.NotEnrolled;

        var sectionProgress = await _unitOfWork.SectionProgress
            .GetAsync(p => p.StudentId == studentId && p.SectionId == request.SectionId);

        if(sectionProgress == null)
        {
            await _unitOfWork.SectionProgress.InsertAsync(new SectionProgress
            {
                SectionId = request.SectionId,
                StudentId = studentId,
                IsCompleted = request.IsCompleted
            });
        }
        else
        {
            sectionProgress.IsCompleted = request.IsCompleted;
        }
        
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success("Section progress updated successfully.");
    }
}
