using LMS.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Courses.Commands.DeleteEnrollment
{
    public class DeleteEnrollmentCommand(int courseId, int studentId) : IRequest<Result>
    {
        public int CourseId { get; } = courseId;
        public int StudentId { get; } = studentId;
    }
}
