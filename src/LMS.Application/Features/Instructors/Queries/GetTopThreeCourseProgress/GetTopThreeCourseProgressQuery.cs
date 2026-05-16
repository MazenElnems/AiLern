using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Instructors.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Instructors.Queries.GetTopThreeCourseProgress;

public class GetTopThreeCourseProgressQuery : IRequest<Result<List<TopThreeCourseProgressDto>>>
{

}
