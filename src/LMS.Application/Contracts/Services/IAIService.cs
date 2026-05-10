using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Models.Responses;
using LMS.Domain.Enums;

namespace LMS.Application.Contracts.ExternalServices;

public interface IAIService
{
    Task<AIUploadDocsResponse> UploadFilesAsContextAsync(List<AIUploadDocsRequest> uploadDocsRequest, CancellationToken cancellationToken);
    Task<AIQuizGenerationResonse> GenerateQuestionsAsync(AIQuizGenerationRequest quizGenerationRequest, CancellationToken cancellationToken);
    Task<AIDeleteProjectResponse> DeleteFileAsync(string projectId, CancellationToken cancellationToken);
    Task<AIGradingResponse> GradeAsync(AIQuizSubmissionGradingRequest aIQuizSubmissionGradingRequest, CancellationToken cancellationToken);
}
