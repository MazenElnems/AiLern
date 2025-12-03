using FluentValidation;
using LMS.Core.Queries.Students.GetMyCoursesQuery;

namespace LMS.Core.Queries.Students.GetAllQuery
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
