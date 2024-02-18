namespace HackerNews.API.Application.Data;

/// <summary>
/// The public DTO used to return top stories to the client with only the required fields
/// </summary>
public record HackerNewsStoryDTO
{
    public required string Title { get; init; }
    public required string Uri { get; init; }
    public required string PostedBy { get; init; }
    public DateTime Time { get; init; }
    public int Score { get; init; }
    public int CommentCount { get; init; }
}