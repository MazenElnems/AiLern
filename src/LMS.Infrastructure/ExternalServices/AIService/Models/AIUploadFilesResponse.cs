namespace LMS.Infrastructure.ExternalServices.AIService.Models;

public class AIUploadFilesResponse
{
    public string Status { get; set; }
    public string Filename { get; set; }
    public int ChunksInserted { get; set; }
    public string ProjectId { get; set; }
}
