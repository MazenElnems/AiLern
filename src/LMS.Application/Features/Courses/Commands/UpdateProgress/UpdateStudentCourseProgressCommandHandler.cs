using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Enums;
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
        var studentId = _userContext.GetCurrentUser().Id;

        if(!await _unitOfWork.Courses.AnyAsync(c => c.Id == request.CourseId))
            return DomainErrors.Course.NotFound(request.CourseId);

        if(!await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, studentId))
            return DomainErrors.Course.NotEnrolled;

        var courseProgress = await _unitOfWork.CourseProgress
            .GetAsync(p => p.StudentId == studentId && p.CourseId == request.CourseId);

        if(courseProgress == null)
        {
            await _unitOfWork.CourseProgress.InsertAsync(new CourseProgress
            {
                CourseId = request.CourseId,
                StudentId = studentId,
                LastOpenedFileId = request.LastOpenedFileId,
                LastWatchedTime = request.LastWatchedTime,
                LastPageNumber = request.LastPageNumber,
                Type = request.LastWatchedTime.HasValue ? LearningType.Video : LearningType.File,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            courseProgress.LastOpenedFileId = request.LastOpenedFileId;
            courseProgress.LastWatchedTime = request.LastWatchedTime;
            courseProgress.LastPageNumber = request.LastPageNumber;
            courseProgress.Type = request.LastWatchedTime.HasValue ? LearningType.Video : LearningType.File;
            courseProgress.UpdatedAt = DateTime.UtcNow;
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success("Progress updated successfully");
    }
}
