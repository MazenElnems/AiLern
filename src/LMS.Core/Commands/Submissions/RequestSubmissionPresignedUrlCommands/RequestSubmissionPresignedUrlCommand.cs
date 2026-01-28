using LMS.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Core.Commands.Submissions.RequestSubmissionPresignedUrlCommands
{
    public class RequestSubmissionPresignedUrlCommand:IRequest<PreSignedUrlResponse>
    {
        public List<FileMetaData> Files { get; set; }
        [JsonIgnore]
        public int SubmissionId { get; set; }

    }
}
