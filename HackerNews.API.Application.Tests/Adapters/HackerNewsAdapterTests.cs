using HackerNews.API.Application.Adapters;
using HackerNews.API.Application.Data;
using HackerNews.API.Application.Mappers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace HackerNews.API.Application.Tests.Adapters;

[TestFixture]
public class HackerNewsAdapterTests
{
    private ILogger<HackerNewsAdapter> _loggerMock;
    private IRestApiAdapter _restApiAdapterMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = Substitute.For<ILogger<HackerNewsAdapter>>();
        _restApiAdapterMock = Substitute.For<IRestApiAdapter>();
    }

    [Test]
    public async Task GetBestStoriesAsync_With_SingleRequest_Returns_SingleStoryDTO()
    {
        var storyItemIds = new int[]{1};
        var expectedTime = DateTimeOffset.UtcNow;
        var expectedItem = new HackerNewsItem { by = "user1", descendants = 10, score = 15, title = "title1", url = "http://www.someurl.com", time = expectedTime.ToUnixTimeSeconds(), id = 1 };

        _restApiAdapterMock.GetAsync<int[]>("topstories.json", CancellationToken.None).Returns(storyItemIds);
        _restApiAdapterMock.GetAsync<HackerNewsItem>($"item/{storyItemIds[0]}.json", CancellationToken.None).Returns(expectedItem);

        var hackerNewsStoryMapper = new HackerNewsStoryMapper();

        var classUnderTest = new HackerNewsAdapter(_loggerMock, _restApiAdapterMock, hackerNewsStoryMapper);

        var result = await classUnderTest.GetBestStoriesAsync(1, CancellationToken.None);
        Assert.That(result.Count, Is.EqualTo(1));

        var hackerNewsStoryDto = result.First();

        Assert.That(hackerNewsStoryDto.Title, Is.EqualTo(expectedItem.title));
        Assert.That(hackerNewsStoryDto.Uri, Is.EqualTo(expectedItem.url));
        Assert.That(hackerNewsStoryDto.PostedBy, Is.EqualTo(expectedItem.by));
        Assert.That(hackerNewsStoryDto.Time, Is.EqualTo(ExpectedTimeLocalDateTime(expectedTime)));
        Assert.That(hackerNewsStoryDto.Score, Is.EqualTo(expectedItem.score));
        Assert.That(hackerNewsStoryDto.CommentCount, Is.EqualTo(expectedItem.descendants));
    }

    [Test]
    public async Task GetBestStoriesAsync_With_MultipleRequests_Returns_OrderedByScore_Items()
    {
        var storyItemIds = new int[] { 1, 2 };
        var expectedTime = DateTimeOffset.UtcNow;
        var expectedItem1 = new HackerNewsItem { by = "user1", descendants = 10, score = 15, title = "title1", url = "http://www.someurl.com", time = expectedTime.ToUnixTimeSeconds(), id = 1 };
        var expectedItem2 = new HackerNewsItem { by = "user2", descendants = 20, score = 20, title = "title2", url = "http://www.someurl2.com", time = expectedTime.ToUnixTimeSeconds(), id = 2 };

        _restApiAdapterMock.GetAsync<int[]>("topstories.json", CancellationToken.None).Returns(storyItemIds);
        _restApiAdapterMock.GetAsync<HackerNewsItem>($"item/{storyItemIds[0]}.json", CancellationToken.None).Returns(expectedItem1);
        _restApiAdapterMock.GetAsync<HackerNewsItem>($"item/{storyItemIds[1]}.json", CancellationToken.None).Returns(expectedItem2);

        var hackerNewsStoryMapper = new HackerNewsStoryMapper();

        var classUnderTest = new HackerNewsAdapter(_loggerMock, _restApiAdapterMock, hackerNewsStoryMapper);

        var result = await classUnderTest.GetBestStoriesAsync(2, CancellationToken.None);
        Assert.That(result.Count, Is.EqualTo(2));

        var hackerNewsStoryDto = result.First();
        Assert.That(hackerNewsStoryDto.Title, Is.EqualTo(expectedItem2.title));
        Assert.That(hackerNewsStoryDto.Uri, Is.EqualTo(expectedItem2.url));
        Assert.That(hackerNewsStoryDto.PostedBy, Is.EqualTo(expectedItem2.by));
        Assert.That(hackerNewsStoryDto.Time, Is.EqualTo(ExpectedTimeLocalDateTime(expectedTime)));
        Assert.That(hackerNewsStoryDto.Score, Is.EqualTo(expectedItem2.score));
        Assert.That(hackerNewsStoryDto.CommentCount, Is.EqualTo(expectedItem2.descendants));

        hackerNewsStoryDto = result.Last();
        Assert.That(hackerNewsStoryDto.Title, Is.EqualTo(expectedItem1.title));
        Assert.That(hackerNewsStoryDto.Uri, Is.EqualTo(expectedItem1.url));
        Assert.That(hackerNewsStoryDto.PostedBy, Is.EqualTo(expectedItem1.by));
        Assert.That(hackerNewsStoryDto.Time, Is.EqualTo(ExpectedTimeLocalDateTime(expectedTime)));
        Assert.That(hackerNewsStoryDto.Score, Is.EqualTo(expectedItem1.score));
        Assert.That(hackerNewsStoryDto.CommentCount, Is.EqualTo(expectedItem1.descendants));
    }


    private static DateTime ExpectedTimeLocalDateTime(DateTimeOffset expectedTime)
    {
        var localDt = expectedTime.LocalDateTime;
        return new DateTime(localDt.Year, localDt.Month, localDt.Day, localDt.Hour, localDt.Minute, localDt.Second, DateTimeKind.Local);
    }
}