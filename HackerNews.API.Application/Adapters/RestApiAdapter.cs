using System.Net;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace HackerNews.API.Application.Adapters;

public interface IRestApiAdapter
{
    Task<TResponse> GetAsync<TResponse>(string requestUri, CancellationToken cancellationToken);
    Task<TResponse> PostAsync<TResponse>(string requestUri, object request, CancellationToken cancellationToken) where TResponse : class;
}

/// <summary>
/// A very simple Rest API adapter to either GET or POST requests to a given base url and specified request Uri
/// </summary>
public class RestApiAdapter : IRestApiAdapter
{
    private readonly string _baseUri;

    public RestApiAdapter(string baseUri)
    {
        _baseUri = baseUri;
    }

    public async Task<TResponse> GetAsync<TResponse>(string requestUri, CancellationToken cancellationToken)
    {
        using (var client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true }))
        {
            client.BaseAddress = new Uri(_baseUri);
            client.DefaultRequestHeaders.Accept.Add(GetJsonMediaType());

            var response = await client.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            // Deserialize the JSON string to the specified type T
            TResponse result = JsonConvert.DeserializeObject<TResponse>(responseContent);
            return result;
        }
    }

    public async Task<TResponse> PostAsync<TResponse>(string requestUri, object request, CancellationToken cancellationToken) where TResponse : class
    {
        using (var client = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true }))
        {
            client.BaseAddress = new Uri(_baseUri);
            client.DefaultRequestHeaders.Accept.Add(GetJsonMediaType());

            var content = GetJsonContent(request);
            var response = await client.PostAsync(requestUri, content, cancellationToken);

            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            // Deserialize the JSON string to the specified type T
            TResponse result = JsonConvert.DeserializeObject<TResponse>(responseContent);
            return result;
        }
    }

    private static MediaTypeWithQualityHeaderValue GetJsonMediaType()
    {
        return new MediaTypeWithQualityHeaderValue("application/json");
    }

    private static HttpContent GetJsonContent(object? request)
    {
        if (request == null) return null;

        var jsonData = JsonConvert.SerializeObject(request);
        return new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
    }
}