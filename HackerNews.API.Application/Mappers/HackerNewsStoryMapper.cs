using HackerNews.API.Application.Data;

namespace HackerNews.API.Application.Mappers;

public interface IHackerNewsStoryMapper
{
    IEnumerable<HackerNewsStoryDTO> MapOut(IEnumerable<HackerNewsItem> newsItems);
}

/// <summary>
/// Maps requests from the underlying firebaseio Hacker News API to the customised Hacker News API
/// </summary>
public class HackerNewsStoryMapper : IHackerNewsStoryMapper
{
    private static readonly DateTime EpocDateTime = new (1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public IEnumerable<HackerNewsStoryDTO> MapOut(IEnumerable<HackerNewsItem> newsItems)
    {
        return newsItems.Select(item => new HackerNewsStoryDTO
        {
            Uri = item.url,
            PostedBy = item.by,
            Time = FromUnixTime(item.time),
            Title = item.title,
            Score = item.score,
            CommentCount = item.descendants
        });
    }


    private static DateTime FromUnixTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        return EpocDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
    }
}