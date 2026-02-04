using FluentValidation;
using LMS.Domain.DTOs;
using LMS.Domain.DTOs.MaterialFiles;
using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Validators
{
    public class MaterialFileMetaDataValidator : AbstractValidator<MaterialFileMetadata>
    {
        private const long MaxFileSizeInBytes = 10 * 1024 * 1024;      // 10 MB
        private const long MaxVideoSizeInBytes = 5L * 1024 * 1024 * 1024;   // 5 GB

        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        // Documents
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "text/plain",

        // Images
        "image/jpeg",
        "image/png",

        // Archives
        "application/zip",

        // Videos
        "video/mp4",
        "video/webm",
        "video/ogg",
        "video/x-msvideo",   // avi
        "video/quicktime",   // mov
        "video/x-matroska"   // mkv
    };

        public MaterialFileMetaDataValidator()
        {
            RuleFor(file => file.FileName)
                .NotEmpty().WithMessage("File name must not be empty.")
                .MaximumLength(255).WithMessage("File name must not exceed 255 characters.")
                .Must(filename => !string.IsNullOrEmpty(Path.GetExtension(filename)))
                .WithMessage("File name must have a valid extension.");

            RuleFor(file => file.ContentType)
                .NotEmpty().WithMessage("Content type must not be empty.")
                .Must(type => AllowedContentTypes.Contains(type))
                .WithMessage("Content type is not allowed.");

            RuleFor(file => file)
                .Must(file =>
                {
                    if (file.ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
                        return file.FileSize > 0 && file.FileSize <= MaxVideoSizeInBytes;

                    return file.FileSize > 0 && file.FileSize <= MaxFileSizeInBytes;
                })
                .WithMessage(file =>
                    file.ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase)
                        ? $"Video size must not exceed {MaxVideoSizeInBytes / (1024*1024 * 1024)} GB."
                        : $"File size must not exceed {MaxFileSizeInBytes / (1024 * 1024)} MB."
                );
        }
    }

}
