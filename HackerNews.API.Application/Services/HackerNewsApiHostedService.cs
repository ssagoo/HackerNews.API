using HackerNews.API.Application.Processors;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HackerNews.API.Application.Services;

/// <summary>
/// A basic hosting service to kick-start any associated Processors so that background threads can be started if needed.
/// </summary>
public class HackerNewsApiHostedService : IHostedService
{
    private readonly ILogger<HackerNewsApiHostedService> _logger;
    private readonly IEnumerable<IProcessor> _processors;

    public HackerNewsApiHostedService(ILogger<HackerNewsApiHostedService> logger, IEnumerable<IProcessor> processors)
    {
        _logger = logger;
        _processors = processors;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var processor in _processors)
        {
            processor.Start(cancellationToken);
        }

        _logger.LogInformation($"Started {nameof(HackerNewsApiHostedService)}");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var processor in _processors)
        {
            processor.Stop();
        }

        _logger.LogInformation($"Stopped {nameof(HackerNewsApiHostedService)}");
        return Task.CompletedTask;
    }
}