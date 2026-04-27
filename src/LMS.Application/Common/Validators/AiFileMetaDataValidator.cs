using FluentValidation;
using LMS.Application.Common.Models.Request;

namespace LMS.Application.Common.Validators;

public class AiFileMetaDataValidator : AbstractValidator<FileMetaData>
{
    private const long MaxFileSizeInBytes = 10 * 1024 * 1024; // 10 MB

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "text/plain"
    };

    public AiFileMetaDataValidator()
    {
        RuleFor(file => file.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .MaximumLength(255).WithMessage("File name must not exceed 255 characters.")
            .Must(name => !string.IsNullOrEmpty(Path.GetExtension(name)))
            .WithMessage("File name must have a valid extension.");

        RuleFor(file => file.ContentType)
            .NotEmpty().WithMessage("Content type is required.")
            .Must(type => AllowedContentTypes.Contains(type))
            .WithMessage("Only PDF, DOCX, and TXT files are allowed.");

        RuleFor(file => file.FileSize)
            .GreaterThan(0).WithMessage("File size must be greater than 0.")
            .LessThanOrEqualTo(MaxFileSizeInBytes)
            .WithMessage($"File size must not exceed {MaxFileSizeInBytes / (1024 * 1024)} MB.");
    }
}