using LMS.Application.Common.Results;
using LMS.Application.Features.Attempts.Shared.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LMS.Application.Features.Attempts.Commands.GradeSubmission;

public class GradeSubmissionCommand : IRequest<Result>
{
    [BindNever]
    public Guid Id { get; set; } 
    public List<GradeSubmissionDto> Grades { get; set; }
}
