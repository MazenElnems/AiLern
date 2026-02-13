namespace LMS.Domain.DTOs.MaterialFiles;

public class MaterialFileMetadataDto : FileMetaData
{

    public DateTime UploadDate { get; set; }
    public int OrderIndex { get; set; }
    public string? FileSource { get; set; }

}
