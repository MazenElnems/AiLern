using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs;
using LMS.Domain.DTOs.MaterialFiles;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.Features.Sections.Commands.RequestPreSignedUrl
{
    public class RequestMaterialPresignedUrlCommand : IRequest<Result<PreSignedUrlResponse>>
    {
        public List<MaterialFileMetadata> Files { get; set; }
        [JsonIgnore]
        public Guid SectionId { get; set; }
    }
}
