// See https://aka.ms/new-console-template for more information

using CommandLine;
using HackerNews.API.Application.Adapters;
using HackerNews.API.Client.Tester;

var options = Parser.Default.ParseArguments<HackerNewsClientOptions>(args).Value;
if (options == null) return;

var cancellationTokenSource = new CancellationTokenSource();

var restApiAdapter = new RestApiAdapter(options.HackerNewsApuBaseUri);
var hackerNewsClient = new HackerNewsClient(restApiAdapter);

var task = hackerNewsClient.GetHackerNewsStories(options.BatchesCount, options.PerBatchStories, cancellationTokenSource.Token);

Console.CancelKeyPress += (sender, e) =>
{
    cancellationTokenSource.Cancel();
    task.Wait();
    e.Cancel = true;
};

Console.WriteLine("Press Ctrl+C to cancel and exit...");
await task;
