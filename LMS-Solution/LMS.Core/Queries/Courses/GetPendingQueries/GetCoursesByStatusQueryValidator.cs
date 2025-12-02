using System.Linq;
using FluentValidation;
using LMS.Domin.Enums;
namespace LMS.Core.Queries.Courses.GetPendingQueries
{
    public class GetCoursesByStatusQueryValidator : AbstractValidator<GetCoursesByStatusQuery>
    {
        public GetCoursesByStatusQueryValidator()
        {
            RuleFor(q => q.PageNumber)
                .GreaterThan(0).WithMessage("page number must positive number");

            RuleFor(q => q.PageSize)
                .GreaterThan(0).WithMessage("page size must positive number");
            RuleFor(u => u.Status)
                .NotEmpty()
                .Must(r => Enum.TryParse<CourseStatus>(r,true, out var status) &&
                    new[] { CourseStatus.Approved, CourseStatus.Rejected, CourseStatus.Pending, CourseStatus.Edited }.Contains(status))
                .WithMessage($"Invalid role. Choose one of the following: " +
                $"{CourseStatus.Approved}, {CourseStatus.Rejected}, or {CourseStatus.Pending}, {CourseStatus.Edited}.");



        }
    }
}
