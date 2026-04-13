using FluentValidation;

namespace LMS.Application.Features.Sections.Commands.UpdateSection;

public class SectionUpdateCommandValidator : AbstractValidator<SectionUpdateCommand>
{
    //private readonly IUnitOfWork _unitOfWork;

    //public SectionUpdateCommandValidator(IUnitOfWork unitOfWork)
    //{
    //    _unitOfWork = unitOfWork;

    //    RuleFor(x => x.SectionNumber)
    //        .GreaterThan(0)
    //        .WithMessage("Section number must be greater than 0.");

    //    RuleFor(x => x)
    //        .MustAsync(BeUniqueSectionNumber)
    //        .WithMessage("Section number already exists in this course.");
    //}

    //private async Task<bool> BeUniqueSectionNumber(
    //    SectionUpdateCommand command,
    //    CancellationToken cancellationToken)
    //{
    //    var section = await _unitOfWork.Sections.GetAsync(sec => sec.Id == command.Id, [nameof(Section.Course)]);
    //    var courseId = section!.CourseId;
    //    return !await _unitOfWork.Sections
    //        .AnyAsync(s =>
    //            s.CourseId == courseId &&
    //            s.SectionNumber == command.SectionNumber,
    //            cancellationToken);
    //}
}
