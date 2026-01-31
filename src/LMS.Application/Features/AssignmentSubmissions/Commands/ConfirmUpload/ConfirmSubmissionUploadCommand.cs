using LMS.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.ConfirmUpload
{
    public class ConfirmSubmissionUploadCommand : IRequest<Result>
    {
        
        public int SubmissionId { get; set; }
    }
}
