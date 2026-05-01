using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Models.Responses;

namespace LMS.Application.Contracts.ExternalServices;

public interface IAIService
{
    public Task<AIUploadFilesResponse> UploadFileAsync(string projectId, string filename, Stream fileStream, CancellationToken cancellationToken);
    public Task<AIUploadDocsResponse> UploadFilesAsContextAsync(List<AIUploadDocsRequest> uploadDocsRequest, CancellationToken cancellationToken);
    public Task<AIQuizGenerationResonse> GenerateQuestionsAsync(AIQuizGenerationRequest quizGenerationRequest, CancellationToken cancellationToken);
    public Task<AIDeleteProjectResponse> DeleteFileAsync(string projectId, CancellationToken cancellationToken);
}
