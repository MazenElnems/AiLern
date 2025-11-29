using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Core.Commands.Courses.RejectEnrollmentCommands
{
    public class RejectEnrollmentCommand(int courseId,int studentId, string reason) : IRequest<string>
    {
        [JsonIgnore]
        public int CourseId { get; set; } = courseId;
        [JsonIgnore]
        public int StudentId { get; set; } = studentId;
        public string Reason { get; set; } = reason;
    }
}
