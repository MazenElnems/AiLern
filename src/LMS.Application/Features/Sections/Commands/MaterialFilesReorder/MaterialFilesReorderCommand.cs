using LMS.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.Features.Sections.Commands.MaterialFilesReorder
{
    public class MaterialFilesReorderCommand : IRequest<Result>
    {
        [JsonIgnore]
        public Guid sectionId { get; set; }
        public List<Guid> OrderedFilesIds { get; set; } = new List<Guid>();
    }
}
