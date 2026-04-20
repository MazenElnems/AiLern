using LMS.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.ReviewSubmission;

public class SubmissionReviewCommand : IRequest<Result>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Feedback {  get; set; }

}
