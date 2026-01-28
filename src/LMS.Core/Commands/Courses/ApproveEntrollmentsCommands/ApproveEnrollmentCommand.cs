using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Commands.Courses.ApproveEntrollmentsCommands
{
    public class ApproveEnrollmentCommand(int courseId,int studentId) : IRequest
    {
        public int CourseId { get; set; } = courseId;
        public int StudentId { get; set; } = studentId;
    }
}
