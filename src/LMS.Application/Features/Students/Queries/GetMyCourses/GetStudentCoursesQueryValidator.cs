using FluentValidation;

namespace LMS.Application.Features.Students.Queries.GetMyCourses
{
    public class GetStudentCoursesQueryValidator : AbstractValidator<GetStudentCoursesQuery>
    {
        public GetStudentCoursesQueryValidator()
        {
            RuleFor(q => q.PageNumber)
    .GreaterThan(0).WithMessage("page number must positive number");

            RuleFor(q => q.PageSize)
                .GreaterThan(0).WithMessage("page size must positive number");


        }
    }
}
