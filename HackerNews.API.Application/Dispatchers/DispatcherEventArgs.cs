namespace HackerNews.API.Application.Dispatchers;

public delegate void DispatcherEventCallback(object sender, IDispatcherEventArgs dispatcherEventArgs);

public interface IDispatcherEventArgs
{
    IDispatchEvent DispatchEvent { get; }
    CancellationToken CancellationToken { get; }
}

/// <summary>
/// Exclusively used by the dispatcher when raising events to the interested processor who needs to handle requests from a controller
/// </summary>
/// <param name="dispatchEvent"></param>
/// <param name="cancellationToken"></param>
public class DispatcherEventArgs(IDispatchEvent dispatchEvent, CancellationToken cancellationToken)
    : EventArgs, IDispatcherEventArgs
{
    public IDispatchEvent DispatchEvent { get; } = dispatchEvent;
    public CancellationToken CancellationToken { get; } = cancellationToken;
}