namespace HackerNews.API.Application.Data;

/// <summary>
/// A cut down response object only containing the required fields as returned by <see href="https://hacker-news.firebaseio.com/v0/item/39412198.json?print=pretty"/>
/// </summary>
public class HackerNewsItem
{
    public string by { get; init; }
    public int descendants { get; init; }
    public int id { get; init; }
    public int score { get; init; }
    public long time { get; init; }
    public string title { get; init; }
    public string url { get; init; }
}