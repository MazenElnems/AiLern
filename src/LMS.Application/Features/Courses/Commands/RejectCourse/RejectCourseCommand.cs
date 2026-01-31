using LMS.Application.Common.Results.Generic;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.Features.Courses.Commands.RejectCourse
{
    public class RejectCourseCommand(int id,string reason) : IRequest<Result<string>>
    {
        [JsonIgnore]
        public int Id { get; set; } = id;
        public string Reason { get; set; } = reason;
    }
}
