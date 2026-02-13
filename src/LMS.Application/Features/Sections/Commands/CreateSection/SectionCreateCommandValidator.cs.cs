using FluentValidation;
using LMS.Domain.Repositories;

namespace LMS.Application.Features.Sections.Commands.CreateSection;

public class CreateSectionCommandValidator : AbstractValidator<SectionCreateCommand>
{

    private readonly IUnitOfWork _unitOfWork;

    public CreateSectionCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.SectionNumber)
            .GreaterThan(0)
            .WithMessage("Section number must be greater than 0.");

        RuleFor(x => x)
            .MustAsync(BeUniqueSectionNumber)
            .WithMessage("Section number already exists in this course.");
    }

    private async Task<bool> BeUniqueSectionNumber(
        SectionCreateCommand command,
        CancellationToken cancellationToken)
    {
        return !await _unitOfWork.Sections
            .AnyAsync(s =>
                s.CourseId == command.CourseId &&
                s.SectionNumber == command.SectionNumber,
                cancellationToken);
    }
}
