using System.Text.Json.Serialization;

namespace LMS.Infrastructure.ExternalServices.AIService.Responses;

public class AIUploadFilesResponse
{
    public string Status { get; set; }
    public string Filename { get; set; }
    public string Message { get; set; }
    public int ChunksInserted { get; set; }
    public string ProjectId { get; set; }
}
