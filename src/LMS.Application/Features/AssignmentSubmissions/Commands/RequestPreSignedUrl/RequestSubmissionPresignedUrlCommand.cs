using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.RequestPreSignedUrl
{
    public class RequestSubmissionPresignedUrlCommand:IRequest<Result<PreSignedUrlResponse>>
    {
        public List<FileMetaData> Files { get; set; }
        [JsonIgnore]
        public int SubmissionId { get; set; }

    }
}
