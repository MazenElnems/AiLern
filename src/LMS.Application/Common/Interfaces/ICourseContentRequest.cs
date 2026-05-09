using MediatR;

namespace LMS.Application.Common.Interfaces;

public interface ICourseContentRequest<TResult> : IRequest<TResult>
{
    public int CourseId { get; }   
}
