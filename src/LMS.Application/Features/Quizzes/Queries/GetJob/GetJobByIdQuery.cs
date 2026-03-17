using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LMS.Application.Features.Quizzes.Queries.GetJob;

public class GetJobByIdQuery : IRequest<Result<GetJobDto>>
{
    [BindNever]
    public Guid Id { get; set; }
}
