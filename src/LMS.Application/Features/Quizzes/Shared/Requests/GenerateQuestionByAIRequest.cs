using LMS.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace LMS.Application.Features.Quizzes.Shared.Requests;

public class GenerateQuestionByAIRequest
{
    public List<Guid> FileIds { get; set; }
    public List<IFormFile> NewUploadedFiles { get; set; }
    public int QuestionsCount { get; set; }
    public Dictionary<QuestionType, int> QuestionTypeCounts { get; set; }
    public Dictionary<QuestionDifficultyLevels, float> QuestionDifficultyPercents { get; set; }
    public string? Query { get; set; }
}
