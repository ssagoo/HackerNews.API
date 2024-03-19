namespace HackerNews.API.Application.Data;

public record GetBestStoriesResultDTO
{
    public IList<HackerNewsStoryDTO> Results { get; init; }
}