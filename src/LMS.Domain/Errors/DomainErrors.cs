using System;

namespace LMS.Domain.Errors;

public static class DomainErrors
{
    public static class Common
    {
        public static Error NotFound(string resource, string identifier) =>
            Error.NotFound($"{resource}.NotFound", $"{resource} with ID: {identifier} not found.");

        public static Error Validation(string title, string message) =>
            Error.Validation(title, message);

        public static Error Unauthorized(string message = "User is not authenticated.") =>
            Error.Unauthorized("Auth.Unauthorized", message);

        public static Error Forbidden(string message) =>
            Error.Forbidden("Auth.Forbidden", message);

        public static Error BusinessRule(string title, string message) =>
            Error.BusinessRule(title, message);

        public static Error Conflict(string title, string message) => 
            Error.Conflict(title, message);
    }

    public static class Course
    {
        public static Error NotFound(int id) =>
            Common.NotFound("Course", id.ToString());

        public static Error NotApproved =>
            Common.Forbidden("You cannot perform this action because the course has not been approved.");

        public static Error NotOwned =>
            Common.Forbidden("You do not have permission to access this course.");

        public static Error AlreadyEnrolled =>
            Common.BusinessRule("Course.AlreadyEnrolled", "Student is already enrolled in this course.");
        public static Error AlreadyExist =>
            Common.BusinessRule("Course.AlreadyExist", "A course with the same code or name already exists.");

        public static Error RejectApproved =>
            Common.BusinessRule("Course.RejectApproved", "Can't reject approved course.");

        public static Error ApproveRejected =>
            Common.BusinessRule("Course.ApproveRejected", "Can't approve already rejected course.");

        public static Error NotEnrolled =>
            Common.Forbidden("You are not enrolled in this course.");
    }

    public static class Enrollment
    {
        public static Error NotFound(string key) =>
            Common.NotFound("Enrollment", key);
    }

    public static class Assignment
    {
        public static Error NotFound(int id) =>
            Common.NotFound("Assignment", id.ToString());

        public static Error NotOwned =>
            Common.Forbidden("You do not have permission to access this assignment.");

        public static Error NotPublished =>
            Common.BusinessRule("Assignment.NotPublished", "Assignment is not published.");

        public static Error InValidDueDate =>
            Common.BusinessRule("Assignment.InValidDueDate", "Due date cannot be earlier than the current date.");
    }
    public static class AiResource
    {
        public static Error NotFound(Guid id) =>
            Common.NotFound("AIResource", id.ToString());

        public static Error ErrorWhileDeletingFile => 
            Common.Conflict("AIResource", "Error occurred while deleting the AI resource.");
    }
    public static class Quiz
    {
        public static Error QuizStarted  =>
            Common.BusinessRule("Quiz.QuizStarted", "Cannot modify the quiz after it has started.");

        public static Error CannotDecreaseMaximumAttempts 
            => Common.BusinessRule("Quiz.CannotDecreaseMaximumAttempts", "Cannot decrease the maximum number of attempts after the quiz has started.");
        public static Error CannotPublishEmptyQuiz
            => Common.BusinessRule("Quiz.CannotPublishEmptyQuiz", "Cannot publish an empty quiz.");
        public static Error CannotDecreaseAttemptTimeLimit
            => Common.BusinessRule("Quiz.CannotDecreaseAttemptTimeLimit", "Cannot decrease the attempt time limit after the quiz has started.");
        public static Error CannotShortenQuizDuration
            => Common.BusinessRule("Quiz.CannotShortenQuizDuration", "Cannot shorten the quiz duration after it has started.");
        public static Error StartTimeCannotBeInThePast
            => Common.BusinessRule("Quiz.StartTimeCannotBeInThePast", "Start time cannot be in the past.");
        public static Error CannotDeleteQuizDuration =>
            Common.BusinessRule("Quiz.CannotDeleteQuizDuration", "Cannot delete quiz after it has started.");
        public static Error UpdateQuestionsAfterQuizStarted =>
            Common.BusinessRule("Quiz.UpdateQuestionsAfterQuizStarted", "Cannot update questions after the quiz has started.");
        public static Error NotFound(Guid id) =>
            Common.NotFound("Quiz", id.ToString());

        public static Error NotOwned =>
            Common.Forbidden("You do not have permission to access this quiz.");

        public static Error NotPublished =>
            Common.Forbidden("Quiz is not published.");

        public static Error InValidDueDate =>
            Common.BusinessRule("Quiz.InValidDueDate", "Due date cannot be earlier than the current date.");
        public static Error AlreadyPublished =>
            Common.BusinessRule("Quiz.AlreadyPublished", "Quiz is already published and cannot be modified.");
        public static Error InvalidAvailabilityRange =>
            Common.BusinessRule(
                "Quiz.InvalidAvailabilityRange",
                "AvailableUntil must be later than AvailableFrom."
            );

        public static Error QuizFinished =>
            Common.BusinessRule("Quiz.QuizFinished", "The quiz has already finished and cannot be modified.");
        public static Error QuizNotAvailableAtThisTime =>
            Common.BusinessRule("Quiz.NotAvailableAtThisTime", "The quiz is not available at the current time.");
    }

    public static class Attempt
    {
        public static Error NotSubmitted =>
            Common.BusinessRule("Attempt.NotSubmitted", "The attempt has not been submitted yet, you cannot view the result.");

        public static Error NotFound(Guid attemptId)
            => Common.NotFound("Attempt", attemptId.ToString());

        public static Error MaximumAttemptsReaches =>
            Common.Forbidden("you exceed the maximum number of attempts.");

        public static Error DuplicateAttempt =>
            Common.Conflict("Attempt.Duplicate","You have already created this attempt.");

        public static Error AnotherAttemptSessionStarted =>
            Common.BusinessRule("Attempt.AnotherAttemptSessionStarted", "cannot start a new attempt, there is an In-Progress Attempt.");

        public static Error NotInProgress =>
            Common.Forbidden("Attempt is not in progress.");

        public static Error TimeExpired =>
            Common.Forbidden("Attempt time has expired.");

        public static Error InvalidQuestion(Guid questionId) =>
            Common.Validation("Attempt.InvalidQuestion", $"Question with ID {questionId} does not belong to this attempt.");

        public static Error StillInProgress =>
            Common.BusinessRule("Attempt.StillInProgress", "The attempt is still in progress and cannot be graded.");

        public static Error NotOwned =>
            Common.Forbidden("You do not have permission to access this attempt.");

        public static Error NotPublished =>
            Common.BusinessRule("Quiz.NotPublished", "Quiz is not published.");

        public static Error QuizNotFinshYet 
            => Common.BusinessRule("Quiz.NotFinshYet", "Quiz is not finished yet, you cannot view the result.");

        public static Error AttemptNotReviewedYet
            => Common.BusinessRule("Attempt.NotReviewedYet", "Attempt is not reviewed yet, you cannot view the result.");
    }

    public static class QuestionGenerationJob
    {
        public static Error NotFound(Guid id) =>
            Common.NotFound("Job", id.ToString());


    }

    public static class QuestionGenerationJobs
    {
        public static Error NotFound(Guid id) =>
            Common.NotFound("Quiz", id.ToString());

        public static Error NotInProgress =>
            Common.BusinessRule("QuestionGenerationJobs.NotInProgress", "QuestionGenerationJob is not In Progress");
        public static Error AlreadyCanceled =>
            Common.BusinessRule("QuestionGenerationJobs.AlreadyCanceled", "QuestionGenerationJob is AlreadyCanceled");
    }

    public static class AssignmentFile
    {
        public static Error NotFound(Guid id) =>
            Common.NotFound("AssignmentFile", id.ToString());
    }
    public static class MaterialFile
    {
        public static Error NotFound(Guid id) =>
            Common.NotFound("MaterialFile", id.ToString());
    }

    public static class AssignmentSubmission
    {
        public static Error NotFound(string id) =>
            Common.NotFound("Submission", id);

        public static Error LateNotAllowed =>
            Common.BusinessRule("Submission.LateNotAllowed", "Late submission is not allowed for this assignment.");

        public static Error DeleteForbidden =>
            Common.Forbidden("You cannot delete another student's submission.");

        public static Error DeleteAfterDeadline =>
            Common.Forbidden("Submission deletion is not allowed after the assignment deadline.");

        public static Error AlreadySubmitted =>
            Common.BusinessRule("Submission.AlreadySubmitted", "You have already submitted this assignment.");

        public static Error SubmissionNotFound =>
            Common.NotFound("Submission.NotFound", "You have not submitted this assignment yet.");
    }

    public static class User
    {
        public static Error NotFound(string id) =>
            Common.NotFound("User", id);

        public static Error AlreadyExists =>
            Common.BusinessRule("User.AlreadyExists", "User with this email is already exists.");

        public static Error RoleAssignmentFailed(string role) =>
            Common.BusinessRule("User.RoleAssignmentFailed", $"Unable to assign user to role {role}");

        public static Error CreationFailed(string message) =>
            Common.BusinessRule("User.CreationFailed", message);
        public static Error InvalidPassword =>
            Common.BusinessRule("InvalidPassword", "The current password is incorrect.");
    }

    public static class Role
    {
        public static Error NotFound(string role) =>
            Common.NotFound("Role", role);

        public static Error RemoveOnlyRole =>
            Common.BusinessRule("Role.RemoveOnlyRole", "Cannot remove this role because it is the user's only role.");

        public static Error RemoveAdminRole =>
            Common.BusinessRule("Role.RemoveAdminRole", "Cannot remove the Admin role from a user.");

        public static Error RemoveFailed(string message) =>
            Common.BusinessRule("Role.RemoveFailed", message);
    }

    public static class Auth
    {
        public static Error InvalidRole =>
            Common.BusinessRule("Auth.InvalidRole", "Invalid user role");
        
        public static Error InvalidCredentials =>
            Common.Unauthorized("Invalid email or password.");

        public static Error EmailNotConfirmed =>
            Common.BusinessRule("Auth.EmailNotConfirmed", "Email is not confirmed.");

        public static Error EmailConfirmationFailed =>
            Common.BusinessRule("Auth.EmailConfirmationFailed", "Email confirmation failed.");

        public static Error PasswordResetFailed =>
            Common.BusinessRule("Auth.PasswordResetFailed", "Can't reset the password, please try again.");

        public static Error ChangePasswordFailed(string message) =>
            Common.BusinessRule("Auth.ChangePasswordFailed", message);

        public static Error RefreshTokenNotFound(string token) =>
            Common.NotFound("RefreshToken", token);
    }

    public static class Pagination
    {
        public static Error InvalidParameters =>
            Common.Validation("Pagination", "PageNumber and PageSize must be greater than zero.");
    }

    public static class Storage
    {
        public static Error FileMissing =>
            Common.Validation("Storage.FileMissing", "File does not exist in storage.");
    }

    public static class Section
    {
        public static Error NotFound(Guid id) =>
            Common.NotFound("Section", id.ToString());

        public static Error NotOwned =>
            Common.Forbidden("You do not have permission to modify this assignment.");

        public static Error NotPublished =>
            Common.BusinessRule("Assignment.NotPublished", "Assignment is not published.");

        public static Error Empty =>
            Common.BusinessRule("Section.Empty", "Section is Empty.");
    }
}
