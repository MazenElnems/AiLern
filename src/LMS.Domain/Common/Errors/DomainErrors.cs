namespace LMS.Domain.Common.Errors;

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
    }

    public static class Course
    {
        public static Error NotFound(int id) =>
            Common.NotFound("Course", id.ToString());

        public static Error NotApproved =>
            Common.BusinessRule("Course.NotApproved", "Cannot enroll in a course that is not approved.");

        public static Error AlreadyEnrolled =>
            Common.BusinessRule("Course.AlreadyEnrolled", "Student is already enrolled in this course.");

        public static Error RejectApproved =>
            Common.BusinessRule("Course.RejectApproved", "Can't reject approved course.");

        public static Error ApproveRejected =>
            Common.BusinessRule("Course.ApproveRejected", "Can't approve already rejected course.");
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
            Common.Forbidden("You do not have permission to modify this assignment.");

        public static Error NotPublished =>
            Common.BusinessRule("Assignment.NotPublished", "Assignment is not published.");
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

    public static class Submission
    {
        public static Error NotFound(string id) =>
            Common.NotFound("Submission", id);

        public static Error NotEnrolled =>
            Common.Forbidden("You are not enrolled in this course.");

        public static Error LateNotAllowed =>
            Common.BusinessRule("Submission.LateNotAllowed", "Late submission is not allowed for this assignment.");

        public static Error DeleteForbidden =>
            Common.Forbidden("You cannot delete another student's submission.");

        public static Error DeleteAfterDeadline =>
            Common.Forbidden("Submission deletion is not allowed after the assignment deadline.");
    }

    public static class User
    {
        public static Error NotFound(string id) =>
            Common.NotFound("User", id);

        public static Error AlreadyExists =>
            Common.BusinessRule("User.AlreadyExists", "User already exists.");

        public static Error CreationFailed(string message) =>
            Common.BusinessRule("User.CreationFailed", message);
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
        public static Error InvalidCredentials =>
            Common.Unauthorized("Invalid email or password.");

        public static Error EmailNotConfirmed =>
            Common.BusinessRule("Auth.EmailNotConfirmed", "Email is not confirmed.");

        public static Error EmailConfirmationFailed =>
            Common.BusinessRule("Auth.EmailConfirmationFailed", "Email confirmation failed.");

        public static Error PasswordResetFailed =>
            Common.BusinessRule("Auth.PasswordResetFailed", "Can't reset the password, please try again.");

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
