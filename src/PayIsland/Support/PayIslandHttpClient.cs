using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using PayIsland.Exceptions;

namespace PayIsland.Support;

internal sealed class PayIslandHttpClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly PayIslandConfig _config;
    private readonly HttpClient _httpClient;

    public PayIslandHttpClient(PayIslandConfig config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
    }

    public Task<Dictionary<string, object?>> GetAsync(string path, CancellationToken cancellationToken)
    {
        return SendAsync(HttpMethod.Get, path, payload: null, cancellationToken);
    }

    public Task<Dictionary<string, object?>> PostAsync(
        string path,
        Dictionary<string, object?> payload,
        CancellationToken cancellationToken)
    {
        return SendAsync(HttpMethod.Post, path, payload, cancellationToken);
    }

    private async Task<Dictionary<string, object?>> SendAsync(
        HttpMethod method,
        string path,
        Dictionary<string, object?>? payload,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, BuildUri(path));
        ApplyHeaders(request);

        if (payload is not null)
        {
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseBody = response.Content is null
            ? null
            : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var responseData = ParseResponseData(responseBody);

        if (!response.IsSuccessStatusCode)
        {
            throw new PayIslandApiException(response.StatusCode, responseBody, responseData);
        }

        return responseData ?? new Dictionary<string, object?>();
    }

    private Uri BuildUri(string path)
    {
        var normalizedPath = path.StartsWith('/') ? path : $"/{path}";
        return new Uri($"{_config.NormalizedBaseUrl}{normalizedPath}", UriKind.Absolute);
    }

    private void ApplyHeaders(HttpRequestMessage request)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config.SecretKey);
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.Clear();
        request.Headers.UserAgent.ParseAdd("payisland-dotnet");
    }

    private static Dictionary<string, object?>? ParseResponseData(string? responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object?>>(responseBody, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
