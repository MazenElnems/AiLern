using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;

namespace LMS.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private CurrentUserIdentity User;

    public PermissionService(IUserContext userContext, IUnitOfWork unitOfWork)
    {
        User = userContext.GetCurrentUser();
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Course>> AuthorizeInstructorAccessToCourseAsync(int courseId)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
        if (course is null)
            return DomainErrors.Course.NotFound(courseId);

        if (course.InstructorId != User.Id)
            return DomainErrors.Course.NotOwned;

        return course;
    }

    public async Task<Result<Quiz>> AuthorizeInstructorAccessToQuizAsync(Guid quizId)
    {
        var quiz = await _unitOfWork.Quizzes.GetAsync(
            q => q.Id == quizId,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz is null)
            return DomainErrors.Quiz.NotFound(quizId);

        if (quiz.Course.InstructorId != User.Id)
            return DomainErrors.Quiz.NotOwned;

        return quiz;
    }

    public async Task<Result<Assignment>> AuthorizeInstructorAccessToAssignmentAsync(int assignmentId)
    {
        var assignment = await _unitOfWork.Assignments.GetAsync(
            a => a.Id == assignmentId,
            includeProperties: [nameof(Assignment.Course)]);

        if (assignment is null)
            return DomainErrors.Assignment.NotFound(assignmentId);

        if (assignment.Course.InstructorId != User.Id)
            return DomainErrors.Assignment.NotOwned;

        return assignment;
    }

    public async Task<Result<Section>> AuthorizeInstructorAccessToSectionAsync(Guid sectionId)
    {
        var section = await _unitOfWork.Sections.GetAsync(
            s => s.Id == sectionId,
            includeProperties: [nameof(Section.Course)]);

        if (section is null)
            return DomainErrors.Section.NotFound(sectionId);

        if (section.Course.InstructorId != User.Id)
            return DomainErrors.Section.NotOwned;

        return section;
    }

    public async Task<Result> AuthorizeStudentEnrollmentAsync(int courseId)
    {
        if (!await _unitOfWork.Enrollments.IsEnrolledAsync(courseId, User.Id))
            return DomainErrors.Course.NotEnrolled;

        return Result.Success();
    }

    public async Task<Result<Attempt>> AuthorizeStudentAccessToAttemptAsync(Guid attemptId)
    {
        var attempt = await _unitOfWork.Attempts.GetByIdAsync(attemptId);

        if (attempt is null)
            return DomainErrors.Attempt.NotFound(attemptId);

        if (attempt.StudentId != User.Id)
            return DomainErrors.Attempt.NotOwned;

        return attempt;
    }

    public async Task<Result<AssignmentSubmission>> AuthorizeStudentAccessToSubmissionAsync(int submissionId)
    {
        var submission = await _unitOfWork.AssignmentSubmissions.GetAsync(
            s => s.Id == submissionId);

        if (submission is null)
            return DomainErrors.AssignmentSubmission.NotFound(submissionId.ToString());

        if (submission.StudentId != User.Id)
            return DomainErrors.Common.Forbidden("You do not have permission to access this submission.");

        return submission;
    }

    public async Task<Result<Course>> AuthorizeCourseAccessAsync(int courseId)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
        if (course is null)
            return DomainErrors.Course.NotFound(courseId);

        if (User.IsInRole(UserRoles.Instructor) && course.InstructorId != User.Id)
            return DomainErrors.Course.NotOwned;

        if (User.IsInRole(UserRoles.Student)
            && !await _unitOfWork.Enrollments.IsEnrolledAsync(courseId, User.Id))
            return DomainErrors.Course.NotEnrolled;

        return course;
    }

    public async Task<Result<Quiz>> AuthorizeQuizAccessAsync(Guid quizId)
    {
        var quiz = await _unitOfWork.Quizzes.GetAsync(
            q => q.Id == quizId,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz is null)
            return DomainErrors.Quiz.NotFound(quizId);

        if (User.IsInRole(UserRoles.Instructor) && quiz.Course.InstructorId != User.Id)
            return DomainErrors.Quiz.NotOwned;

        if (User.IsInRole(UserRoles.Student)
            && !await _unitOfWork.Enrollments.IsEnrolledAsync(quiz.CourseId, User.Id))
            return DomainErrors.Course.NotEnrolled;

        return quiz;
    }

    public async Task<Result<Assignment>> AuthorizeAssignmentAccessAsync(int assignmentId)
    {
        var assignment = await _unitOfWork.Assignments.GetAsync(
            a => a.Id == assignmentId,
            includeProperties: [nameof(Assignment.Course)]);

        if (assignment is null)
            return DomainErrors.Assignment.NotFound(assignmentId);

        if (User.IsInRole(UserRoles.Instructor) && assignment.Course.InstructorId != User.Id)
            return DomainErrors.Assignment.NotOwned;

        if (User.IsInRole(UserRoles.Student)
            && !await _unitOfWork.Enrollments.IsEnrolledAsync(assignment.CourseId, User.Id))
            return DomainErrors.Course.NotEnrolled;

        return assignment;
    }

    public async Task<Result<Section>> AuthorizeSectionAccessAsync(Guid sectionId)
    {
        var section = await _unitOfWork.Sections.GetAsync(
            s => s.Id == sectionId,
            includeProperties: [nameof(Section.Course)]);

        if (section is null)
            return DomainErrors.Section.NotFound(sectionId);

        if (User.IsInRole(UserRoles.Instructor) && section.Course.InstructorId != User.Id)
            return DomainErrors.Section.NotOwned;

        if (User.IsInRole(UserRoles.Student)
            && !await _unitOfWork.Enrollments.IsEnrolledAsync(section.CourseId, User.Id))
            return DomainErrors.Course.NotEnrolled;

        return section;
    }
}
