using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs.MaterialFiles;
using MediatR;

namespace LMS.Application.Features.Sections.Queries.GetSectionFiles;

public record GetSectionFilesQuery(Guid sectionId) : IRequest<Result<List<MaterialFileMetadataDto>>>;
