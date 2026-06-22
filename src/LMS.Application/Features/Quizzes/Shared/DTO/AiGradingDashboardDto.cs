namespace LMS.Application.Features.Quizzes.Shared.DTO;

public class AiGradingDashboardDto
{
    public int TotalAiEvaluation { get; set; }
    public double AverageAiRating { get; set; }
    public int SatisfacationRate { get; set; }
    public int LowQualityReviews { get; set; }
    public int PoorCount { get; set; }
    public int FairCount { get; set; }
    public int GoodCount { get; set; }
    public int VeryGoodCount { get; set; }
    public int ExcellentCount { get; set; }
    public Dictionary<string, int> InstructorFeedbackOnAiGrading { get; set; }
    public List<LowestRated> LowestRatedAiEvaluations { get; set; }

}

public class LowestRated
{
    public int Rating { get; set; }
    public string QuestionText { get; set; }
    public string CourseName { get; set; }
    public double? AiScore { get; set; }
    public string AiFeedback { get; set; }
    public string InstructorName { get; set; }

}