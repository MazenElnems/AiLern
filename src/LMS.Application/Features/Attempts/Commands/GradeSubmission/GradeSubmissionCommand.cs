using LMS.Application.Common.Results;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Attempts.Commands.GradeSubmission;

public class GradeSubmissionCommand: IRequest<Result>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public List<GradeSubmissionDto> Grades { get; set; }
    public AttemptStatus Status { get; set; }
}
