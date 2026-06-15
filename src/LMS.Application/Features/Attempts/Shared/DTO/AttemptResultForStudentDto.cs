namespace LMS.Application.Features.Attempts.Shared.DTO;

public class AttemptResultForStudentDto
{
    public List<AttemptQuestionDto> Answers { get; set; }
    public List<string> WeakTopics { get; set; }    
}
