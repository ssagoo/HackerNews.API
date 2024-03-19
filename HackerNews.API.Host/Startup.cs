using HackerNews.API.Application.Adapters;
using HackerNews.API.Application.Controllers;
using HackerNews.API.Application.Dispatchers;
using HackerNews.API.Application.Events;
using HackerNews.API.Application.Mappers;
using HackerNews.API.Application.Middleware;
using HackerNews.API.Application.Processors;
using HackerNews.API.Application.Settings;

namespace HackerNews.API.Host;

public class Startup
{
    private readonly IConfiguration _config;
    private readonly string _policyName = "CorsPolicy";

    public Startup(IConfiguration config)
    {
        _config = config;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var appSettings = _config.GetSection("appSettings");
        var newsApiSettings = appSettings.Get<HackerNewsApiSettings>();
        if (newsApiSettings == null) throw new ArgumentNullException(nameof(newsApiSettings));

        services.AddKeyedSingleton<IDispatcher, SingleThreadedDispatcher>("appDispatcher");
        services.AddSingleton<IRestApiAdapter, RestApiAdapter>(_ => new RestApiAdapter(newsApiSettings.HackerNewApiUrl));
        services.AddSingleton<IHackerNewsStoryMapper, HackerNewsStoryMapper>();
        services.AddSingleton<IHackerNewsAdapter, HackerNewsAdapter>();
        services.AddSingleton<IHackerNewsApiRequestLimiter, HackerNewsApiRequestLimiter>(provider 
            => new HackerNewsApiRequestLimiter(newsApiSettings.MaxConcurrentRequests));

        services.AddSingleton<IHackerNewsEventDelegate, HackerNewsEventDelegateHandler>();
        services.AddSingleton<IProcessor, HackerNewsProcessor>();

        services.AddCors(o => o.AddPolicy(_policyName, builder =>
        {
            /*
            builder.WithOrigins("http://localhost")
                .AllowAnyHeader()
                .AllowAnyMethod();*/

            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }));

        services.AddControllers().AddApplicationPart(typeof(HackerNewsApiController).Assembly);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime, ILoggerFactory loggerFactory)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseCors(_policyName);

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}