using HackerNews.API.Application.Adapters;
using HackerNews.API.Application.Mappers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace HackerNews.API.Application.IntegrationTests.Adapters;

[TestFixture]
public class HackerNewsAdapterTests
{
    private const string HackerNewsApiBaseUri = "https://hacker-news.firebaseio.com/v0/";

    private ILogger<HackerNewsAdapter> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = Substitute.For<ILogger<HackerNewsAdapter>>();
    }

    [Test]
    public async Task GetBestStoriesAsync_With_SingleCount()
    {
        var apiAdapter = new RestApiAdapter(HackerNewsApiBaseUri);
        var hackerNewsStoryMapper = new HackerNewsStoryMapper();
        IHackerNewsAdapter hackerNewsAdapter = new HackerNewsAdapter(_loggerMock, apiAdapter, hackerNewsStoryMapper);

        var results = await hackerNewsAdapter.GetBestStoriesAsync(bestStoriesCount:1, CancellationToken.None);
        
        Assert.That(results.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetBestStoriesAsync_With_MultipleCount()
    {
        var apiAdapter = new RestApiAdapter(HackerNewsApiBaseUri);
        var hackerNewsStoryMapper = new HackerNewsStoryMapper();
        IHackerNewsAdapter hackerNewsAdapter = new HackerNewsAdapter(_loggerMock, apiAdapter, hackerNewsStoryMapper);

        var results = await hackerNewsAdapter.GetBestStoriesAsync(bestStoriesCount: 5, CancellationToken.None);

        Assert.That(results.Count, Is.EqualTo(5));
    }
}