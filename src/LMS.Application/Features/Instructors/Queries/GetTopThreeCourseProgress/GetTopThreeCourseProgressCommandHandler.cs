//using LMS.Application.Common.Results.Generic;
//using LMS.Application.Contracts.UnitOfWork;
//using LMS.Application.CurrentUser;
//using LMS.Application.Features.Instructors.Shared.DTO;
//using MediatR;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;

//namespace LMS.Application.Features.Instructors.Queries.GetTopThreeCourseProgress;

//public class GetTopThreeCourseProgressCommandHandler : IRequestHandler<GetTopThreeCourseProgressCommand, List<TopThreeCourseProgressDto>>
//{
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly IUserContext _user;
//    public GetTopThreeCourseProgressCommandHandler(IUnitOfWork unitOfWork, IUserContext user)
//    {
//        _unitOfWork = unitOfWork;
//        _user = user;
//    }

//    public Task<List<TopThreeCourseProgressDto>> Handle(GetTopThreeCourseProgressCommand request, CancellationToken cancellationToken)
//    {
//        var userId = _user.GetCurrentUser().Id;
//        var course = _unitOfWork.Courses.Query
//            .AsNoTracking()
//            .Where(c => c.InstructorId == userId)
//            .Select(c => new TopThreeCourseProgressDto
//            {
//                CourseName = c.Name,
//                //ProgressPercentage = c.Progresses.Count > 0 ? (double)c.Progresses.Count(p => p. >= 100) / c.Progresses.Count * 100 : 0,
//                StudentsCount = c.Enrollments.Count,
//                QuizzesCount = c.Quizzes.Count
//            })
//            .OrderByDescending(c => c.ProgressPercentage)
//            .Take(3)
//            .ToList();
//        return Result<List<TopThreeCourseProgressDto>>.Success(course);
//    }
//}
