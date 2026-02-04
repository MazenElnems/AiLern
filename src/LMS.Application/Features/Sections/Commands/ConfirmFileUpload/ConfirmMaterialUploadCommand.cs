using LMS.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Sections.Commands.ConfirmFileUpload
{
    public class ConfirmMaterialUploadCommand : IRequest<Result>
    {
        public Guid SectionId { get; set; }
    }
}
