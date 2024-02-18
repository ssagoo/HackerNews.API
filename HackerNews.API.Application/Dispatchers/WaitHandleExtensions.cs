namespace HackerNews.API.Application.Dispatchers;

/// <summary>
/// Extensions to support async/await on a ManualResetEvent used by the dispatcher events
/// </summary>
public static class WaitHandleExtensions
{
    public static Task<object> AsTask(this IDispatchEvent dispatchEvent)
    {
        return AsTask(dispatchEvent, Timeout.InfiniteTimeSpan);
    }

    public static Task<object> AsTask(this IDispatchEvent dispatchEvent, TimeSpan timeout)
    {
        var tcs = new TaskCompletionSource<object>();
        var registration = ThreadPool.RegisterWaitForSingleObject(dispatchEvent.WaitHandle, (state, timedOut) =>
        {
            var localTcs = (TaskCompletionSource<object>)state;
            if (timedOut)
                localTcs.TrySetCanceled();
            else
            {
                localTcs.TrySetResult(dispatchEvent.Response);
            }
        }, tcs, timeout, executeOnlyOnce: true);
        tcs.Task.ContinueWith((_, state) => ((RegisteredWaitHandle)state).Unregister(null), registration, TaskScheduler.Default);
        return tcs.Task;
    }
}