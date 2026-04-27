using LMS.Application.Common.Results;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Sections.Commands.UpdateSection;

public class SectionUpdateCommand : IRequest<Result>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Title { get; set; }

}
