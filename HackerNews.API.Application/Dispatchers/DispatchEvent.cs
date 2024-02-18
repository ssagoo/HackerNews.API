namespace HackerNews.API.Application.Dispatchers;


public interface IDispatchEvent : IDisposable
{
    IEvent Event { get; }
    object Request { get; }
    object Response { get; }
    Exception Error { get; }
    WaitHandle WaitHandle { get; }
    void SetResponse(object response);
    void SetError(Exception error);
    void Wait();
    Task<object> WaitAsync(TimeSpan timeout);
}

/// <summary>
/// Used by the EventDelegate and Dispatcher when enqueueing new requests in an orderly manner from Controller => Dispatcher => Processor
/// </summary>
public class DispatchEvent : IDispatchEvent
{
    public IEvent Event { get; }
    public object Request { get; }
    public object Response { get; private set; }
    public Exception Error { get; private set; }
    public WaitHandle WaitHandle => _eventSink.WaitHandle;

    private readonly ManualResetEventSlim _eventSink;

    public DispatchEvent(IEvent @event, object request)
    {
        Event = @event;
        Request = request;
        _eventSink = new ManualResetEventSlim(false);
    }

    public void SetResponse(object response)
    {
        Response = response;
        _eventSink.Set();
    }

    public void SetError(Exception error)
    {
        Error = error;
        _eventSink.Set();
    }

    public void Wait()
    {
        _eventSink.Wait();
    }

    public Task<object> WaitAsync(TimeSpan timeout)
    {
        return this.AsTask(timeout);
    }


    public void Dispose()
    {
        _eventSink?.Dispose();
    }
}