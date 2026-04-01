using AutoMapper;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Instructors.Shared.DTO;
using LMS.Domain.Enums;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Instructors.Queries.GetUpcomingEvents;

public class GetUpcomingEventsQueryHandler : IRequestHandler<GetUpcomingEventsQuery, Result<PaginationResult<UpcomingEventsDto>>>
{
    private readonly IUserContext _user;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetUpcomingEventsQueryHandler> _logger;

    public GetUpcomingEventsQueryHandler(IUserContext user, IUnitOfWork unitOfWork, ILogger<GetUpcomingEventsQueryHandler> logger)
    {
        _user = user;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PaginationResult<UpcomingEventsDto>>> Handle(GetUpcomingEventsQuery request, CancellationToken cancellationToken)
    {
        try
        {


            var userId = _user.GetCurrentUser().Id;
            var events = _unitOfWork.Courses.Query
                 .AsNoTracking()
                 .Where(c => c.InstructorId == userId)
                 .SelectMany(c => c.Quizzes
                     .Where(q => q.AvailableUntil > DateTime.UtcNow)
                     .Select(q => new UpcomingEventsDto
                     {
                         CourseName = c.Name,
                         Title = q.Title,
                         AvailableUntil = q.AvailableUntil,
                         EventType = EventType.Quiz
                     }))
                 .Concat(
                     _unitOfWork.Courses.Query
                         .Where(c => c.InstructorId == userId)
                         .SelectMany(c => c.Assignments
                             .Where(a => a.DueDate > DateTime.UtcNow)
                             .Select(a => new UpcomingEventsDto
                             {
                                 CourseName = c.Name,
                                 Title = a.Title,
                                 AvailableUntil = a.DueDate,
                                 EventType = EventType.Assignment
                             }))
                 );



            if (request.EventType != null)
            {
                events = events
                    .Where(e => e.EventType == request.EventType);
            }
            var totalResult = await events.CountAsync(cancellationToken);

            var upcomingEvents = await events
                .OrderBy(e => e.AvailableUntil)
                .Skip((request.PageNo - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginationResult<UpcomingEventsDto>(
                request.PageNo,
                request.PageSize,
                totalResult,
                upcomingEvents
               );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching upcoming events for instructor with ID {InstructorId}", _user.GetCurrentUser().Id);
            throw;
        }
    }
}
