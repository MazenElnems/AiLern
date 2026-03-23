using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Attempts.Shared.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LMS.Application.Features.Attempts.Queries.GetAttempt;

public class GetAttemptByIdForStudentQuery : IRequest<Result<GetAttemptByIdDto>>
{
    [BindNever]
    public Guid Id { get; set; }
}
