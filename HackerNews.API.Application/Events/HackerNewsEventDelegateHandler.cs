using HackerNews.API.Application.Data;
using HackerNews.API.Application.Dispatchers;
using Microsoft.Extensions.DependencyInjection;

namespace HackerNews.API.Application.Events;

public interface IHackerNewsEventDelegate : IEventDelegate
{
    Task<IList<HackerNewsStoryDTO>> GetBestStories(string userName, int storiesCount);
}

/// <summary>
/// A concrete event delegate to enqueue hacker news api requests into the dispatcher for processing
/// </summary>
/// <param name="dispatcher"></param>
public class HackerNewsEventDelegateHandler([FromKeyedServices("appDispatcher")] IDispatcher dispatcher)
    : EventDelegateHandler(dispatcher), IHackerNewsEventDelegate
{
    public Task<IList<HackerNewsStoryDTO>> GetBestStories(string userName, int storiesCount)
    {
        var request = (userName, storiesCount);
        return DispatchAsync<IList<HackerNewsStoryDTO>, (string userName, int storiesCount)>(HackerNewsEvents.GetStoriesEvent, request);
    }
}
