using LMS.Application.Features.Instructors.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Instructors.Queries.GetTopThreeCourseProgress;

public class GetTopThreeCourseProgressCommand : IRequest<List<TopThreeCourseProgressDto>>
{

}
