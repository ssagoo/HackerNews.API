using HackerNews.API.Application.Adapters;
using HackerNews.API.Application.Dispatchers;
using HackerNews.API.Application.Events;
using Microsoft.Extensions.DependencyInjection;

namespace HackerNews.API.Application.Processors;

public interface IHackerNewsProcessor : IProcessor
{
}

/// <summary>
/// The concrete Hacker News API Processor to handle incoming requests from the Controller and delegate them via the IHackerNewsAdapter class
/// </summary>
public class HackerNewsProcessor : BaseProcessor, IHackerNewsProcessor
{
    private readonly IHackerNewsAdapter _hackerNewsAdapter;

    public HackerNewsProcessor([FromKeyedServices("appDispatcher")] IDispatcher dispatcher, IHackerNewsAdapter hackerNewsAdapter) : base(dispatcher)
    {
        _hackerNewsAdapter = hackerNewsAdapter;
    }

    protected override async Task<object> Handle(IEvent @event, CancellationToken cancellationToken, object request)
    {
        switch (@event.EventId)
        {
            case HackerNewsEvents.GetStoriesEventId:
                (string userName, int storiesCount) requestData = ((string userName, int storiesCount))request;

                var bestStories = await _hackerNewsAdapter.GetBestStoriesAsync(requestData.storiesCount, cancellationToken);
                return bestStories;
            default:
                throw new NotSupportedException($"Event with Id {@event.EventId} not supported.");
        }
    }
}