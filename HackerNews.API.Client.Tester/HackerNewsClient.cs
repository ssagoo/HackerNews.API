using System.Diagnostics;
using HackerNews.API.Application.Adapters;
using HackerNews.API.Application.Data;

namespace HackerNews.API.Client.Tester;

public class HackerNewsClient
{
    private readonly IRestApiAdapter _restApiAdapter;

    public HackerNewsClient(IRestApiAdapter restApiAdapter)
    {
        _restApiAdapter = restApiAdapter;
    }

    public async Task<(long total, int errorCount)> GetHackerNewsStories(int batchesCount, int perBatchStories, CancellationToken cancellationToken)
    {
        long totalStories = 0;
        int errorCount = 0;

        await Parallel.ForEachAsync(Enumerable.Range(0, batchesCount), cancellationToken, async (i, token) =>
        {
            try
            {
                if (cancellationToken.IsCancellationRequested) return;

                var sw = Stopwatch.StartNew();
                Console.WriteLine($"Sending request id: {i}");

                await Task.Delay(TimeSpan.FromMicroseconds(100), token);

                var byBatchStories =
                    await _restApiAdapter.GetAsync<ICollection<HackerNewsStoryDTO>>($"beststories?maxStories={perBatchStories}", cancellationToken);

                sw.Stop();
                Console.WriteLine($"Sending Completed for request id: {i} returned '{byBatchStories.Count}' stories, taking: {sw.ElapsedMilliseconds}ms");
                Interlocked.Add(ref totalStories, byBatchStories.Count);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Got error code: {e.StatusCode}, with message: {e.Message}, help text: {e.HelpLink}");
                Interlocked.Increment(ref errorCount);
            }
        });

        Console.WriteLine($"Completed all requests - total stories: {totalStories}, total errors: {errorCount}.");

        return (totalStories, errorCount);
    }
}