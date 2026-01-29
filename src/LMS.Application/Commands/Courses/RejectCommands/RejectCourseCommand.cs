using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.Commands.Courses.RejectCommands
{
    public class RejectCourseCommand(int id,string reason) : IRequest<string>
    {
        [JsonIgnore]
        public int Id { get; set; } = id;
        public string Reason { get; set; } = reason;
    }
}
