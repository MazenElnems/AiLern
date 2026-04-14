namespace LMS.Domain.Entities.Notification;

public enum NotificationType
{
    // Student Notifications
    NewAssignmentAdded,
    CourseMaterialsUpdated,
    NewQuizAdded,
    AttemptReviewed,

    // Instructor Notifications
    DeadlineReached,
    AiQuestionGenerationFinished,
    CourseRemovedByAdmin    
}