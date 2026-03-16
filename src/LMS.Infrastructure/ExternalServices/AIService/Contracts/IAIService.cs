using LMS.Infrastructure.ExternalServices.AIService.Requests;
using LMS.Infrastructure.ExternalServices.AIService.Responses;

namespace LMS.Infrastructure.ExternalServices.AIService.Contracts;

public interface IAIService
{
    public Task<AIUploadFilesResponse> UploadFileAsync(string projectId, string filename, Stream fileStream, CancellationToken cancellationToken);
    public Task<AIQuizGenerationResonse> GenerateQuestionsAsync(AIQuizGenerationRequest quizGenerationRequest, CancellationToken cancellationToken);
}
