namespace LMS.Application.Common.Models.Responses;

public class AIUploadDocsResponse
{
    public string Status { get; set; }
    public string Message { get; set; }
    public int ChunksInserted { get; set; }
    public List<string> UploadedProjectsIds { get; set; }
}
