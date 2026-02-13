namespace LMS.Domain.DTOs.MaterialFiles;

public class MaterialFileMetadataDto 
{
    public DateTime UploadDate { get; set; }
    public int OrderIndex { get; set; }
    public string? FileSource { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; } 
    public string ContentType { get; set; }
}
