using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LMS.Application.Common.Models.Request;

public class AIUploadDocsRequest
{
    public string PresignedUrl { get; set; }
    public string ProjectId { get; set; }   
}