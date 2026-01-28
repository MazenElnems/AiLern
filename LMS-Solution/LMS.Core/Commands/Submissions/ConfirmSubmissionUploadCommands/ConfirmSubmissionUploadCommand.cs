using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Commands.Submissions.ConfirmSubmissionUploadCommands
{
    public class ConfirmSubmissionUploadCommand : IRequest
    {
        
        public int SubmissionId { get; set; }
    }
}
