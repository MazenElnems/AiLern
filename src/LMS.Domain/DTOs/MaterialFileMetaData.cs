using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Domain.DTOs
{
    public class MaterialFileMetadata : FileMetaData
    {
        [JsonIgnore]
        public DateTime UploadDate { get; set; }
        [JsonIgnore]
        public int OrderIndex { get; set; }
    }
}
