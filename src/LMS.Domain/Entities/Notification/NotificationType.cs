namespace LMS.Domain.Entities.Notification;

public enum NotificationType
{
    // Student Notifications
    NewAssignmentAdded,
    CourseMaterialsUpdated,
    NewQuizAdded,
    AttemptReviewed,
    EnrolledInNewCourse,

    // Instructor Notifications
    DeadlineReached,
    AiQuestionGenerationFinished,
    CourseRemovedByAdmin    
}