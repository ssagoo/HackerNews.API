using CommandLine;

namespace HackerNews.API.Client.Tester;

public class HackerNewsClientOptions
{
    [Option('u', "baseUri", Required = false, HelpText = "Base Url for the Hacker News API Host", Default = "http://localhost:5270/api/")]
    public string HackerNewsApuBaseUri { get; set; }

    [Option('c', "count", Required = false, HelpText = "Total number of concurrent requests to send", Default = 10)]
    public int BatchesCount { get; set; }

    [Option('s', "stories", Required = false, HelpText = "Per batch number of stories", Default = 5)]
    public int PerBatchStories { get; set; }
}