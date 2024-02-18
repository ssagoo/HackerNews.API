namespace HackerNews.API.Application.Data;

/// <summary>
/// This DTO is used to return error information when a request has failed
/// </summary>
public record HackerNewsApiRequestFailedDTO
{
    public string Reason { get; init; }
}