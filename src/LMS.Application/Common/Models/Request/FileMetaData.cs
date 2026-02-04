namespace LMS.Application.Common.Models.Request;

public class FileMetaData
{
    public string FileName { get; set; }
    public long FileSize { get; set; } // In bytes
    public string ContentType { get; set; } // Mime type ex: application/pdf
}
