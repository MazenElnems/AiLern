using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Quizzes.Queries.GetAllQuizzes;

public class GetAllQuizzesByCourseIdQueryHandler : IRequestHandler<GetAllQuizzesByCourseIdQuery, Result<PaginationResult<GetAllQuizDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public GetAllQuizzesByCourseIdQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<PaginationResult<GetAllQuizDto>>> Handle(GetAllQuizzesByCourseIdQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var course = await _unitOfWork.Courses.GetAsync(c => c.Id == request.CourseId,
            includeProperties: [nameof(Course.Quizzes)]);

        if (course == null)
            return Result<PaginationResult<GetAllQuizDto>>.Failure(DomainErrors.Course.NotFound(request.CourseId));

        if (user.IsInRole(UserRoles.Instructor) && user.Id != course.InstructorId)
            return Result<PaginationResult<GetAllQuizDto>>.Failure(DomainErrors.Course.NotOwned);

        else if (user.IsInRole(UserRoles.Student) && !await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, user.Id))
            return Result<PaginationResult<GetAllQuizDto>>.Failure(DomainErrors.Common.Forbidden("Can't access this course"));

        var query = _unitOfWork.Quizzes.Query
            .Where(q => q.CourseId == request.CourseId);

        if (user.IsInRole(UserRoles.Student))
            query = query.Where(q => q.Status == QuizStatus.Published);

        var totalResult = await query.CountAsync(cancellationToken);

        var quizzes = await query
            .Select(q => new GetAllQuizDto
            {
                Id = q.Id,
                Title = q.Title,
                Description = q.Description!,
                AvailableFrom = q.AvailableFrom,
                AvailableUntil = q.AvailableUntil,
                Status = q.Status,
                CreatedAt = q.CreatedAt,
                AttemptTimeLimit = q.AttemptTimeLimit,
                ShowResultOnClose = q.ShowResultOnClose,
                MaximumAttempts = q.MaximumAttempts,
                QuestionsCount = q.Questions.Count(),
                StudentAttemptCount = user.IsInRole(UserRoles.Student) ? q.Attempts.Count(a => a.StudentId == user.Id) : q.Attempts.Count(),
                HasActiveAttempt = q.Attempts.Any(a => a.StudentId == user.Id && a.Status == AttemptStatus.InProgress)
            })
            .Skip(request.PageSize * (request.PageNo - 1))
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginationResult<GetAllQuizDto>(
            request.PageNo,
            request.PageSize,
            totalResult,
            quizzes
        );
    }
}
