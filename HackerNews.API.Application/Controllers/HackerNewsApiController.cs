using HackerNews.API.Application.Data;
using HackerNews.API.Application.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HackerNews.API.Application.Controllers
{
    /// <summary>
    /// The main Hacker News API controller as exposed to clients for request top n stories
    /// </summary>
    [ApiController]
    [Route("api")]
    public class HackerNewsApiController : ControllerBase
    {
        private readonly ILogger<HackerNewsApiController> _logger;
        private readonly IHackerNewsEventDelegate _hackerNewsEventDelegate;
        private readonly IHackerNewsApiRequestLimiter _hackerNewsApiRequestLimiter;

        public HackerNewsApiController(ILogger<HackerNewsApiController> logger, IHackerNewsEventDelegate hackerNewsEventDelegate, IHackerNewsApiRequestLimiter hackerNewsApiRequestLimiter)
        {
            _logger = logger;
            _hackerNewsEventDelegate = hackerNewsEventDelegate;
            _hackerNewsApiRequestLimiter = hackerNewsApiRequestLimiter;
        }

        [HttpGet("beststories")]
        public async Task<ActionResult<IList<HackerNewsStoryDTO>>> GetBestStories([FromQuery] int maxStories)
        {
            _hackerNewsApiRequestLimiter.IncRequest(HackerNewsEvents.GetStoriesEventId);
            try
            {
                if (!_hackerNewsApiRequestLimiter.CanAcceptRequestById(HackerNewsEvents.GetStoriesEventId))
                {
                    return GetStatusCode(StatusCodes.Status503ServiceUnavailable, "Hacker News API is busy, please try again later");
                }

                var currentUser = CurrentUserName;
                var bestStories = await _hackerNewsEventDelegate.GetBestStories(currentUser, maxStories);
                if (bestStories == null)
                {
                    return GetStatusCode(StatusCodes.Status503ServiceUnavailable, "Hacker News API is unavailable, please try again later");
                }

                _logger.LogInformation($"Successfully processed request for user: '{currentUser}' from: {ClientIPAddress}, returning ({bestStories.Count}) stories");
                return new ActionResult<IList<HackerNewsStoryDTO>>(bestStories);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, $"Failed to handle request {nameof(GetBestStories)} due to underlying http request error, status: {e.StatusCode} and reason: {e.Message}");
                return GetStatusCode(StatusCodes.Status503ServiceUnavailable, e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to handle request {nameof(GetBestStories)}, reason: {e.Message}");
                return GetStatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            finally
            {
                _hackerNewsApiRequestLimiter.DecRequest(HackerNewsEvents.GetStoriesEventId);
            }
        }

        private ObjectResult GetStatusCode(int statusCode, string reason)
        {
            return StatusCode(statusCode, new HackerNewsApiRequestFailedDTO { Reason = reason });
        }

        private string CurrentUserName => HttpContext.User?.Identity?.Name ?? "Anonymous";

        public string ClientIPAddress
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.Headers["X-Forwarded-For"]))
                {
                    return Request.Headers["X-Forwarded-For"];
                }
                else
                {
                    var feature = Request.HttpContext.Features.Get<IHttpConnectionFeature>();
                    if (feature is { RemoteIpAddress: { } }) return feature.RemoteIpAddress.ToString();
                }

                return HttpContext.Connection.RemoteIpAddress?.ToString();
            }
        }
    }
}
