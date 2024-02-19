using HackerNews.API.Application.Adapters;
using HackerNews.API.Application.Data;
using HackerNews.API.Application.Dispatchers;
using HackerNews.API.Application.Events;
using HackerNews.API.Application.Processors;
using NSubstitute;
using NUnit.Framework;

namespace HackerNews.API.Application.IntegrationTests.Processors;

[TestFixture]
public class HackerNewsProcessorTests
{
    private IDispatcher _mockDispatcher;
    private IHackerNewsAdapter _mockHackerNewsAdapter;

    [SetUp]
    public void Setup()
    {
        _mockDispatcher = Substitute.For<IDispatcher>();
        _mockHackerNewsAdapter = Substitute.For<IHackerNewsAdapter>();
    }

    [Test]
    public async Task Handle_WithValidEventId_Calls_NewsAdapter()
    {
        (string userName, int storiesCount) requestData = ("", 10);
        var expectedStoryDtos = new List<HackerNewsStoryDTO>();

        _mockHackerNewsAdapter.GetBestStoriesAsync(requestData.storiesCount, CancellationToken.None).Returns(expectedStoryDtos);

        var hackerNewsProcessor = new HackerNewsProcessor(_mockDispatcher, _mockHackerNewsAdapter);

        var result = await hackerNewsProcessor.Handle(HackerNewsEvents.GetStoriesEvent, CancellationToken.None, requestData);

        Assert.That(result, Is.EqualTo(expectedStoryDtos));
        await _mockHackerNewsAdapter.Received().GetBestStoriesAsync(requestData.storiesCount, CancellationToken.None);
    }

    [Test]
    public void Handle_WithInvalidEventId_Raises_Exception()
    {
        (string userName, int storiesCount) requestData = ("", 10);

        var hackerNewsProcessor = new HackerNewsProcessor(_mockDispatcher, _mockHackerNewsAdapter);

        Assert.ThrowsAsync<NotSupportedException>(async () => await hackerNewsProcessor.Handle(new Event(-1), CancellationToken.None, requestData));
    }
}