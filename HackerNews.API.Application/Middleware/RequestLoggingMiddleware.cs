using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace HackerNews.API.Application.Middleware;

/// <summary>
/// A basic middle-ware to record time taken to process incoming requests
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
    }

    public async Task Invoke(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            _logger.LogInformation(
                "Request {method} {url} => {statusCode} - duration ({duration:g})",
                context.Request?.Method,
                context.Request?.Path.Value,
                context.Response?.StatusCode,
                sw.Elapsed);
        }
    }
}