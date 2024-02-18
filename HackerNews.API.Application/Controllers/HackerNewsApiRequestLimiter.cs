using System.Collections.Concurrent;

namespace HackerNews.API.Application.Controllers;

public interface IHackerNewsApiRequestLimiter
{
    void IncRequest(int requestId);
    void DecRequest(int requestId);
    bool CanAcceptRequestById(int requestId);
}

/// <summary>
/// A very basic request rate limiter using a counter to maintain current number of in-process requests
/// Can also use a more advanced rate limiter strategy as specified here <see href="https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-8.0"/>
/// </summary>
public class HackerNewsApiRequestLimiter : IHackerNewsApiRequestLimiter
{
    private const int DefaultMaxConcurrentRequests = 5;
    private readonly int _maxConcurrentRequests;
    private readonly ConcurrentDictionary<int, int> _requestCounts;

    public HackerNewsApiRequestLimiter(int? maxConcurrentRequests)
    {
        _maxConcurrentRequests = maxConcurrentRequests ?? DefaultMaxConcurrentRequests;
        _requestCounts = new ConcurrentDictionary<int, int>();
    }

    public void IncRequest(int requestId)
    {
        _requestCounts.AddOrUpdate(requestId, 1, (_, oldValue) => ++oldValue);
    }

    public void DecRequest(int requestId)
    {
        _requestCounts.AddOrUpdate(requestId, 1, (_, oldValue) => --oldValue);
    }

    public bool CanAcceptRequestById(int requestId)
    {
        return _requestCounts.GetOrAdd(requestId, (key) => 0) <= _maxConcurrentRequests;
    }
}