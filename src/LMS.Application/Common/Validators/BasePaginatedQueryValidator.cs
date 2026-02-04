using FluentValidation;
using LMS.Application.Common.Models.Request;

namespace LMS.Application.Common.Validators;

public class BasePaginatedQueryValidator<TQuery> : AbstractValidator<TQuery>
    where TQuery : BasePaginatedQuery
{
    public BasePaginatedQueryValidator()
    {
        RuleFor(q => q.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(q => q.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .Must(p => new List<int> { 5, 10, 20, 30 }.Contains(p)).WithMessage("Page size must be one of the following values: 5, 10, 20, 30.");
    }
}
