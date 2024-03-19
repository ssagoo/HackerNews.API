using HackerNews.API.Application.Adapters;
using HackerNews.API.Application.Controllers;
using HackerNews.API.Application.Dispatchers;
using HackerNews.API.Application.Events;
using HackerNews.API.Application.Mappers;
using HackerNews.API.Application.Processors;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace HackerNews.API.Application.IntegrationTests.Controllers;

[TestFixture]
public class HackerNewsApiControllerTests
{
    private const string HackerNewsApiBaseUri = "https://hacker-news.firebaseio.com/v0/";

    private ILogger<HackerNewsApiController> _mockCtrlLogger;
    private ILogger<SingleThreadedDispatcher> _mockDispLogger;
    private ILogger<HackerNewsAdapter> _mockAdapterLogger;

    [SetUp]
    public void Setup()
    {
        _mockCtrlLogger = Substitute.For<ILogger<HackerNewsApiController>>();
        _mockDispLogger = Substitute.For<ILogger<SingleThreadedDispatcher>>();
        _mockAdapterLogger = Substitute.For<ILogger<HackerNewsAdapter>>();
    }


    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(20)]
    public async Task Given_HackerNewsAPIController_WithMaxStoryCountsAndNoErrors_When_GetBestStories_Then_Return_Stories(int maxStories)
    {
        var apiController = CreateDefaultController(out var processor);
        await processor.Start(CancellationToken.None);
        
        var result = await apiController.GetBestStories(maxStories);

        Assert.That(result.Value.Results.Count, Is.EqualTo(maxStories));
    }


    private HackerNewsApiController CreateDefaultController(out IProcessor processor)
    {
        using var dispatcher = new SingleThreadedDispatcher(_mockDispLogger);
        var newsApiRequestLimiter = new HackerNewsApiRequestLimiter(1);
        var eventDelegateHandler = new HackerNewsEventDelegateHandler(dispatcher);
        var restApiAdapter = new RestApiAdapter(HackerNewsApiBaseUri);
        var hackerNewsStoryMapper = new HackerNewsStoryMapper();
        processor = new HackerNewsProcessor(dispatcher, new HackerNewsAdapter(_mockAdapterLogger, restApiAdapter, hackerNewsStoryMapper));

        var apiController = new HackerNewsApiControllerStub(_mockCtrlLogger, eventDelegateHandler, newsApiRequestLimiter);

        return apiController;
    }

    public class HackerNewsApiControllerStub : HackerNewsApiController
    {
        public HackerNewsApiControllerStub(ILogger<HackerNewsApiController> logger, IHackerNewsEventDelegate hackerNewsEventDelegate, IHackerNewsApiRequestLimiter hackerNewsApiRequestLimiter) 
            : base(logger, hackerNewsEventDelegate, hackerNewsApiRequestLimiter)
        {
        }

        public override string CurrentUserName => "Anonymous";
        public override string ClientIPAddress => "127.0.0.1";
    }
}