using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Core.Commands.Courses.ApproveCommands
{
    public class ApproveCourseCommand(int id): IRequest
    {
        [JsonIgnore]
        public int Id { get; set; } = id;
    }
}
