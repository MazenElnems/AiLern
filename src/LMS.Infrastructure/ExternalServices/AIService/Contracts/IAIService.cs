using LMS.Infrastructure.ExternalServices.AIService.Models;
using LMS.Infrastructure.ExternalServices.AIService.Requests;

namespace LMS.Infrastructure.ExternalServices.AIService.Contracts;

public interface IAIService
{
    public Task<AIUploadFilesResponse> UploadFileAsync(string projectId, string filename, Stream fileStream);
    public Task<AIQuizGenerationResonse> GenerateQuestionsAsync(AIQuizGenerationRequest quizGenerationRequest);
}
