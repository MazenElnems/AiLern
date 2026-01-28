using FluentValidation;
using LMS.Domain.DTOs;

namespace LMS.Core.Validators;

public class FileMetaDataValidator : AbstractValidator<FileMetaData>
{
    private readonly static int MaxFileSizeInBytes = 10 * 1024 * 1024; // 10 MB
    private readonly List<string> allowedContentTypes = new()
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "image/jpeg",
        "image/png",
        "text/plain",
        "application/zip"
    };
    public FileMetaDataValidator()
    {
        RuleFor(file => file.FileName)
            .NotEmpty().WithMessage("File name must not be empty.")
            .MaximumLength(255).WithMessage("File name must not exceed 255 characters.");

        RuleFor(file => file.FileSize)
            .GreaterThan(0).WithMessage("File size must be greater than 0 bytes.")
            .LessThanOrEqualTo(MaxFileSizeInBytes).WithMessage($"File size must not exceed {MaxFileSizeInBytes / (1024 * 1024)} MB.");

        RuleFor(file => file.ContentType)
            .NotEmpty().WithMessage("Content type must not be empty.")
            .Must(contentType => allowedContentTypes.Contains(contentType))
            .WithMessage("Content type is not allowed.");

        RuleFor(file => file.FileName)
            .Must(filename => !string.IsNullOrEmpty(Path.GetExtension(filename)))
            .WithMessage("File name must have a valid extension.");
    }
}
