using HackerNews.API.Application.Dispatchers;

namespace HackerNews.API.Application.Events;

/// <summary>
/// A place-holder class which contains all the known hacker news events that can be raised by the controller
/// </summary>
public static class HackerNewsEvents
{
    public const int BaseEventId = 0;

    public const int GetStoriesEventId = BaseEventId + 1;
    public static readonly Event GetStoriesEvent = new(GetStoriesEventId);
}