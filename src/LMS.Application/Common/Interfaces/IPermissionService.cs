using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Quizzes;

namespace LMS.Application.Common.Interfaces;

public interface IPermissionService
{
    Task<Result<Course>> AuthorizeInstructorAccessToCourseAsync(int courseId);
    Task<Result<Quiz>> AuthorizeInstructorAccessToQuizAsync(Guid quizId);
    Task<Result<Assignment>> AuthorizeInstructorAccessToAssignmentAsync(int assignmentId);
    Task<Result<Section>> AuthorizeInstructorAccessToSectionAsync(Guid sectionId);

    Task<Result> AuthorizeStudentEnrollmentAsync(int courseId);

    Task<Result<Attempt>> AuthorizeStudentAccessToAttemptAsync(Guid attemptId);
    Task<Result<AssignmentSubmission>> AuthorizeStudentAccessToSubmissionAsync(int submissionId);

    Task<Result<Course>> AuthorizeCourseAccessAsync(int courseId);
    Task<Result<Quiz>> AuthorizeQuizAccessAsync(Guid quizId);
    Task<Result<Assignment>> AuthorizeAssignmentAccessAsync(int assignmentId);
    Task<Result<Section>> AuthorizeSectionAccessAsync(Guid sectionId);
}
