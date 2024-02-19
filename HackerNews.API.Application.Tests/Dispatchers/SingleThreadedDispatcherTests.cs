using HackerNews.API.Application.Dispatchers;
using HackerNews.API.Application.Events;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace HackerNews.API.Application.Tests.Dispatchers;

[TestFixture]
public class SingleThreadedDispatcherTests
{
    private ILogger<SingleThreadedDispatcher> _mockDispLogger;

    [SetUp]
    public void Setup()
    {
        _mockDispLogger = Substitute.For<ILogger<SingleThreadedDispatcher>>();
    }

    [Test]
    public void Enqueue_WithRegisteredHandler_Returns_Response()
    {
        using var singleThreadedDispatcher = new SingleThreadedDispatcher(_mockDispLogger);
        singleThreadedDispatcher.Start(CancellationToken.None);

        int expectedRequest = 5;
        int expectedResponse = 10;

        singleThreadedDispatcher.HandleEvent += (sender, args) =>
        {
            Assert.That(args.DispatchEvent.Request, Is.EqualTo(expectedRequest));
            args.DispatchEvent.SetResponse(expectedResponse);
        };

        var dispatchEvent = new DispatchEvent(HackerNewsEvents.GetStoriesEvent, expectedRequest);
        singleThreadedDispatcher.Enqueue(dispatchEvent);

        dispatchEvent.Wait();

        Assert.That(dispatchEvent.Response, Is.EqualTo(expectedResponse));
    }

    [Test]
    public void Enqueue_WithNoRegisteredHandler_Returns_ErrorResponse()
    {
        using var singleThreadedDispatcher = new SingleThreadedDispatcher(_mockDispLogger);
        singleThreadedDispatcher.Start(CancellationToken.None);

        int expectedRequest = 5;
        var dispatchEvent = new DispatchEvent(HackerNewsEvents.GetStoriesEvent, expectedRequest);
        singleThreadedDispatcher.Enqueue(dispatchEvent);

        dispatchEvent.Wait();

        Assert.That(dispatchEvent.Request, Is.EqualTo(expectedRequest));
        Assert.That(dispatchEvent.Response, Is.Null);
        Assert.That(dispatchEvent.Error, Is.TypeOf<InvalidOperationException>());
    }
}