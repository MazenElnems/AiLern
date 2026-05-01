namespace LMS.Application.Common.Models.Responses;

public class AIDeleteProjectResponse
{
    public string Status { get; set; }
    public string ProjectId { get; set; }
    public int CunksDeleted { get; set; }
    public string Message { get; set; }
}
