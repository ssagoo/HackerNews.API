namespace HackerNews.API.Application.Settings;

/// <summary>
/// Encapsulate any app settings required by the Hacker New API hosting service
/// </summary>
public class HackerNewsApiSettings
{
    public string HackerNewApiUrl { get; set; }
    public int? MaxConcurrentRequests { get; set; }
}