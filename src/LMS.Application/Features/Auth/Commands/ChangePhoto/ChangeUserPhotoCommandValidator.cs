using FluentValidation;

namespace LMS.Application.Features.Auth.Commands.ChangePhoto;

public class ChangeUserPhotoCommandValidator : AbstractValidator<ChangeUserPhotoCommand>
{
    public ChangeUserPhotoCommandValidator()
    {
        RuleFor(x => x.Image)
            .NotNull().WithMessage("Image is required.");

        RuleFor(x => x.Image.FileSize)
            .LessThanOrEqualTo(2 * 1024 * 1024) 
            .WithMessage("Image size must not exceed 2MB.");

        RuleFor(x => x.Image.ContentType)
            .Must(type => type == "image/jpeg" || type == "image/png")
            .WithMessage("Only JPEG and PNG images are allowed.");

        RuleFor(x => x.Image.FileName)
            .NotEmpty().WithMessage("File name is required.");
    }
}
