namespace LMS.Application.Features.Dashboards.Shared.DTO;

public class QuizDashboardDto
{
    public int StudentsInCourse { get; set; }
    public int? NumberOfStudents { get; set; }
    public AverageScore? AverageScore { get; set; }
    public Dictionary<string,QuizPassFailDto>? PassesFalis { get; set; }
    public List<QuestionStatisticsDto>? QuestionStatistics { get; set; }
    public List<AttemptStatisticsDto>? AttemptsDistributions { get; set; }
    public List<SubmissionTimeBucketDto>? SubmissionTimeDistribution { get; set; }

}

public class QuizPassFailDto
{
    public int Passes { get; set; }
    public int Fails { get; set; }
}

public class QuestionStatisticsDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; }
    public int CorrectAnswersCount { get; set; }
}
public class SubmissionTimeBucketDto
{
    public int BucketIndex { get; set; } // 1,2,3,4
    public string Label { get; set; }
    public int SubmissionsCount { get; set; }
}

public class AttemptStatisticsDto
{
    public int AttemptNumber { get; set; }
    public int StudentsCount { get; set; }
}

public class AverageScore
{
    public double? MinAverage { get; set; }
    public double? AvgAverage { get; set; }
    public double? MaxAverage { get; set; }
}
