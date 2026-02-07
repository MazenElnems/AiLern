using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Domain.DTOs.MaterialFiles
{
    public class MaterialFileMetadataDto : FileMetaData
    {

        public DateTime UploadDate { get; set; }
        public int OrderIndex { get; set; }
        public string FileSource { get; set; }

    }
}
