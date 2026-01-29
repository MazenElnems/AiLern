using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Commands.Courses.DeleteEnrollmentCommands
{
    public class DeleteEnrollmentCommand(int courseId, int studentId) : IRequest
    {
        public int CourseId { get; } = courseId;
        public int StudentId { get; } = studentId;
    }
}
