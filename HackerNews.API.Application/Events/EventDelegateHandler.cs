using HackerNews.API.Application.Dispatchers;

namespace HackerNews.API.Application.Events;

public interface IEventDelegate
{
    Task<TResponse> DispatchAsync<TResponse, TRequest>(IEvent @event, TRequest request);
    Task<(TResponse response, Exception error)> TryDispatchAsync<TResponse, TRequest>(IEvent @event, TRequest request);
}

/// <summary>
/// A mediator between the creator of an event and the actioner so that events can flow from a Controller to an associated Processor via the Dispatcher
/// concrete classes are required to enqueue new events on the dispatcher which has been registered with an associated processor class
/// </summary>
/// <param name="dispatcher"></param>
public abstract class EventDelegateHandler(IDispatcher dispatcher) : IEventDelegate
{
    public async Task<TResponse> DispatchAsync<TResponse, TRequest>(IEvent @event, TRequest request)
    {
        using var dispatchEvent = new DispatchEvent(@event, request);
        if (!dispatcher.Enqueue(dispatchEvent))
        {
            return default;
        }

        var result = await dispatchEvent.WaitAsync(Timeout.InfiniteTimeSpan);
        if (dispatchEvent.Error != null)
        {
            throw dispatchEvent.Error;
        }

        return result == null ? default : (TResponse)result;
    }

    public async Task<(TResponse response, Exception error)> TryDispatchAsync<TResponse, TRequest>(IEvent @event, TRequest request)
    {
        using var dispatchEvent = new DispatchEvent(@event, request);
        if (!dispatcher.Enqueue(dispatchEvent))
        {
            return default;
        }

        var result = await dispatchEvent.WaitAsync(Timeout.InfiniteTimeSpan);

        return (result == null ? default : (TResponse)result, dispatchEvent.Error);
    }
}