namespace HackerNews.API.Application.Data;

public record HackerNewsApiRequestFailedDTO
{
    public string Reason { get; init; }
    public string Error { get; init; }
}