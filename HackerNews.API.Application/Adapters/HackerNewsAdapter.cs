using System.Collections.Concurrent;
using System.Diagnostics;
using HackerNews.API.Application.Data;
using HackerNews.API.Application.Mappers;
using Microsoft.Extensions.Logging;

namespace HackerNews.API.Application.Adapters;

public interface IHackerNewsAdapter
{
    Task<ICollection<HackerNewsStoryDTO>> GetBestStoriesAsync(int bestStoriesCount, CancellationToken cancellationToken);
}

/// <summary>
/// The main hacker new adapter to talk to the underlying Hacker API as hosted by <see href="https://hacker-news.firebaseio.com/v0/"/>
/// </summary>
public class HackerNewsAdapter : IHackerNewsAdapter
{
    private const int MaxStories = 500;
    private const int StoriesRequestBatchSize = 10; // TODO: Add to app config
    private const string TopStoriesRequestJson = "topstories.json";

    private readonly ILogger<HackerNewsAdapter> _logger;
    private readonly IRestApiAdapter _hackerNewsApi;
    private readonly IHackerNewsStoryMapper _hackerNewsStoryMapper;

    public HackerNewsAdapter(ILogger<HackerNewsAdapter> logger, IRestApiAdapter hackerNewsApi, IHackerNewsStoryMapper hackerNewsStoryMapper)
    {
        _logger = logger;
        _hackerNewsApi = hackerNewsApi;
        _hackerNewsStoryMapper = hackerNewsStoryMapper;
    }


    public async Task<ICollection<HackerNewsStoryDTO>> GetBestStoriesAsync(int bestStoriesCount, CancellationToken cancellationToken)
    {
        bestStoriesCount = Math.Min(MaxStories, bestStoriesCount);

        var topStories = await _hackerNewsApi.GetAsync<int[]>(TopStoriesRequestJson, cancellationToken);
        var topStoriesReduced = bestStoriesCount == topStories.Length ? topStories : topStories.Take(bestStoriesCount).ToArray();

        _logger.LogInformation("Getting '{stories}' stories from hacker news API in batches of {size}", topStoriesReduced.Length, StoriesRequestBatchSize);

        // Get data in batches of specified size: StoriesRequestBatchSize
        var newsItems = new ConcurrentBag<HackerNewsItem>();
        int currentTotal = 0;
        foreach (var batch in topStoriesReduced.Chunk(StoriesRequestBatchSize))
        {
            var sw = Stopwatch.StartNew();
            await Parallel.ForEachAsync(batch, cancellationToken, async (id, token) =>
            {
                if (token.IsCancellationRequested) return;
                var story = await _hackerNewsApi.GetAsync<HackerNewsItem>($"item/{id}.json", cancellationToken);
                newsItems.Add(story);
            });
            currentTotal += batch.Length;

            sw.Stop();
            _logger.LogInformation("Got '{current}' of '{total}', taking: {time-taken}ms", currentTotal, topStoriesReduced.Length, sw.ElapsedMilliseconds);
        }

        // ensure returned data has been ordered by Score descending
        return _hackerNewsStoryMapper.MapOut(newsItems)
            .OrderByDescending(n => n.Score)
            .ToList();
    }
}