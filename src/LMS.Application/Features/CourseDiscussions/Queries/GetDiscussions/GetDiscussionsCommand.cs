using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.CourseDiscussions.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.CourseDiscussions.Queries.GetDiscussions;

public class GetDiscussionsCommand : IRequest<Result<List<DiscussionDto>>>
{
    public int CourseId { get; set; }

}
