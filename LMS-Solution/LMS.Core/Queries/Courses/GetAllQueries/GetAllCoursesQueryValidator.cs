using FluentValidation;

namespace LMS.Core.Queries.Courses.GetAllQueries;

public class GetAllCoursesQueryValidator : AbstractValidator<GetAllCoursesQuery>
{
    public GetAllCoursesQueryValidator()
    {
        RuleFor(q => q.PageNumber)
            .GreaterThan(0).WithMessage("page number must positive number");

        RuleFor(q => q.PageSize)
            .GreaterThan(0).WithMessage("page size must positive number");




    }
}
