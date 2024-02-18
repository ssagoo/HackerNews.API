using HackerNews.API.Application.Services;

namespace HackerNews.API.Host;

public static class StartupExtensions
{
    public static IHostBuilder CreateHost(this string[] args) =>
        Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureLogging((_, loggerCfg) =>
            {
                loggerCfg.ClearProviders();
                loggerCfg.AddConsole();
            })
            .ConfigureServices(services =>
            {
                services.AddHostedService<HackerNewsApiHostedService>();
            })
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
}