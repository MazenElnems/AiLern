using FluentValidation;

namespace LMS.Application.Features.Courses.Queries.GetAllCourses;

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
