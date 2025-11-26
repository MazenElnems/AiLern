using FluentValidation;
using LMS.Core.Queries.Courses.GetAllQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Queries.Courses.GetApprovedQueries
{
    public class GetApprovedCoursesQueryValidator : AbstractValidator<GetApprovedCoursesQuery>
    {
        public GetApprovedCoursesQueryValidator()
        {
            RuleFor(q => q.PageNumber)
                .GreaterThan(0).WithMessage("page number must positive number");

            RuleFor(q => q.PageSize)
                .GreaterThan(0).WithMessage("page size must positive number");
        }
    }
}
