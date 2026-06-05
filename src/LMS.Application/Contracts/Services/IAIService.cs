using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Models.Responses;
using LMS.Domain.Enums;

namespace LMS.Application.Contracts.ExternalServices;

public interface IAIService
{
    public Task<AIUploadDocsResponse> UploadDocsAsContextAsync(List<AIUploadDocsRequest> uploadDocsRequest, CancellationToken cancellationToken);
    public Task<AIQuizGenerationResonse> GenerateQuestionsAsync(AIQuizGenerationRequest quizGenerationRequest, CancellationToken cancellationToken);
    public Task<AIDeleteProjectResponse> DeleteFileAsync(string projectId, CancellationToken cancellationToken);
    public Task<AIBatchGradingResponse> GradeQuizBatchAsync(AIGradingRequest gradingRequest, CancellationToken cancellationToken);
}
