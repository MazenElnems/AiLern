using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Courses.Commands.UpdateProgress;

public class UpdateStudentCourseProgressCommandHandler : IRequestHandler<UpdateStudentCourseProgressCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public UpdateStudentCourseProgressCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(UpdateStudentCourseProgressCommand request, CancellationToken cancellationToken)
    {
        //var studentId = _userContext.GetCurrentUser().Id;

        //if (!await _unitOfWork.Courses.AnyAsync(c => c.Id == request.CourseId))
        //    return DomainErrors.Course.NotFound(request.CourseId);

        //if (!await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, studentId))
        //    return DomainErrors.Course.NotEnrolled;

        //var totalSections = await _unitOfWork.Sections.CountAsync(s => s.CourseId == request.CourseId);
        //var completedSections = totalSections == 0
        //    ? 0
        //    : Math.Min(request.CompletedSections, totalSections);
        //var percent = totalSections == 0
        //    ? 0
        //    : (double)completedSections / totalSections * 100.0;
        //var isCompleted = totalSections > 0 && completedSections == totalSections;

        //var progress = await _unitOfWork.Progresses.GetAsync(
        //    p => p.CourseId == request.CourseId && p.StudentId == studentId);

        //if (progress == null)
        //{
        //    progress = new CourseProgress
        //    {
        //        CourseId = request.CourseId,
        //        StudentId = studentId,
        //        IsCompleted = isCompleted,
        //        UpdatedAt = DateTime.UtcNow,
        //        Percent = percent,
        //        LastPageNumber = request.LastPageNumber,
        //        LastWatchedTime = request.LastWatchedTime,
        //        LastOpenedFileId = request.LastLearningItemId,
        //        Type = request.Type
        //    };
        //    await _unitOfWork.Progresses.InsertAsync(progress);
        //}
        //else
        //{
        //    progress.IsCompleted = isCompleted;
        //    progress.UpdatedAt = DateTime.UtcNow;
        //    progress.Percent = percent;
        //    progress.LastPageNumber = request.LastPageNumber;
        //    progress.LastWatchedTime = request.LastWatchedTime;
        //    progress.LastOpenedFileId = request.LastLearningItemId;
        //    progress.Type = request.Type;
        //}

        //await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success("Progress updated successfully");

    }
}
