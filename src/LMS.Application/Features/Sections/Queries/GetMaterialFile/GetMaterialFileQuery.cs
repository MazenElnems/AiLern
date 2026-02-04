using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs.MaterialFiles;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Sections.Queries.GetMaterialFile
{
    public class GetMaterialFileQuery : IRequest<Result<MaterialFileMetadataDto>>
    {
        public Guid Id { get; set; }
        public Guid FileId { get; set; }
    }
}
