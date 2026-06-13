using LMS.Domain.Enums;

namespace LMS.Application.Features.Instructors.Shared.DTO;

public class UpcomingEventsDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string Title { get; set; }        
    public DateTime AvailableUntil { get; set; }   
    public EventType EventType { get; set; }

}
