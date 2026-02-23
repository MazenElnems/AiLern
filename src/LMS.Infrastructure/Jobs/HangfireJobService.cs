using Hangfire;
using LMS.Domain.Interfaces;
using System.Linq.Expressions;

namespace LMS.Infrastructure.Jobs;

internal class HangfireJobService : IBackgroundService
{
    public void Delete(string jobId)
        => BackgroundJob.Delete(jobId);

    public string Enqueue(Expression<Action> methodCall)
        => BackgroundJob.Enqueue(methodCall);

    public string Enqueue<T>(Expression<Action<T>> methodCall)
        => BackgroundJob.Enqueue(methodCall);

    public string Enqueue(Expression<Func<Task>> methodCall)
        => BackgroundJob.Enqueue(methodCall);

    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
        => BackgroundJob.Enqueue(methodCall);

    public string Schedule(Expression<Action> methodCall, TimeSpan delay)
        => BackgroundJob.Schedule(methodCall, delay);

    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
        => BackgroundJob.Schedule(methodCall, delay);

    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay)
        => BackgroundJob.Schedule(methodCall, delay);

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
        => BackgroundJob.Schedule(methodCall, delay);
}
