namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class AiQuestionGenerationDashboardDto
{
    public int TotalValidation { get; set; }
    public int TopicAlignmentRate { get; set; }
    public int RelatedQuestions { get; set; }
    public int UnrelatedQuestions { get; set; }
    public List<QuestionValidatioOverviewByCourse> OverviewByCourses { get; set;}

}

public class QuestionValidatioOverviewByCourse
{
    public string CourseName { get; set; }
    public int GeneratedByAi { get; set; }
    public int RelatedCount { get; set; }
    public int UnRelatedCount { get; set; }
}
