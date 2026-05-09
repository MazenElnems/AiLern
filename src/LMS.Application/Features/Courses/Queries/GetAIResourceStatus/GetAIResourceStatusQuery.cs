using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Courses.Queries.GetAIResourceStatus;

public record GetAIResourceStatusQuery(int CourseId) : IRequest<Result<List<AIStatusDto>>>;