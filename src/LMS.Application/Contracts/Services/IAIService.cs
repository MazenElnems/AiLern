using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Models.Responses;

namespace LMS.Application.Contracts.ExternalServices;

public interface IAIService
{
    public Task<AIUploadDocsResponse> UploadDocsAsContextAsync(List<AIUploadDocsRequest> uploadDocsRequest, CancellationToken cancellationToken);
    public Task<AIQuizGenerationResonse> GenerateQuestionsAsync(AIQuizGenerationRequest quizGenerationRequest, CancellationToken cancellationToken);
    public Task<AIDeleteProjectResponse> DeleteFileAsync(string projectId, CancellationToken cancellationToken);
}
