using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Instructors.Shared.DTO;
using LMS.Domain.Enums;
using MediatR;

namespace LMS.Application.Features.Instructors.Queries.GetUpcomingEvents;

public record GetUpcomingEventsQuery(EventType? EventType ,int PageNo, int PageSize) :
    IRequest<Result<PaginationResult<UpcomingEventsDto>>>
{

}
