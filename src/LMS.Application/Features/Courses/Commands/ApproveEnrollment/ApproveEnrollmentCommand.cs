using LMS.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Courses.Commands.ApproveEnrollment
{
    public class ApproveEnrollmentCommand(int courseId,int studentId) : IRequest<Result>
    {
        public int CourseId { get; set; } = courseId;
        public int StudentId { get; set; } = studentId;
    }
}
