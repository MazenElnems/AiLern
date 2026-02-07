using System.Text.Json.Serialization;

namespace LMS.Domain.DTOs;

// add these two values when add new matrials 
// and delete this type becasue ew dosn't use it.

public class MaterialFileMetadata : FileMetaData
{
    [JsonIgnore]
    public DateTime UploadDate { get; set; }
    [JsonIgnore]
    public int OrderIndex { get; set; }
}
