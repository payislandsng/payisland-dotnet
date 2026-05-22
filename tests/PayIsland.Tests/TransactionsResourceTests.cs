using System.Net;
using System.Text;
using System.Text.Json;
using PayIsland;
using PayIsland.Exceptions;
using Xunit;

namespace PayIsland.Tests;

public sealed class TransactionsResourceTests
{
    [Fact]
    public async Task InitializeAsync_SendsPostToInitializeEndpoint()
    {
        var handler = new RecordingHandler(_ => JsonResponse("""{"status":"success"}"""));
        var client = CreateClient(handler);

        var response = await client.Transactions.InitializeAsync(new Dictionary<string, object?>
        {
            ["amount"] = "1000",
            ["channel"] = "card"
        });

        Assert.Equal("success", GetString(response, "status"));
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Equal("https://ags.payislands.com/api/v1/transactions/in/initialize", handler.LastRequest.Uri.ToString());
        Assert.Equal("Bearer", handler.LastRequest.AuthorizationScheme);
        Assert.Equal("test_secret_key", handler.LastRequest.AuthorizationParameter);
        Assert.Equal("application/json", handler.LastRequest.ContentType);
        Assert.Contains("payisland-dotnet", handler.LastRequest.UserAgent);
        Assert.Contains("\"amount\":\"1000\"", handler.LastRequest.Body);
    }

    [Fact]
    public async Task VerifyAsync_SendsGetToVerifyEndpoint()
    {
        var handler = new RecordingHandler(_ => JsonResponse("""{"status":"success"}"""));
        var client = CreateClient(handler);

        await client.Transactions.VerifyAsync("order_123");

        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Get, handler.LastRequest.Method);
        Assert.Equal("https://ags.payislands.com/api/v1/transactions/in/check-transaction-status/order_123", handler.LastRequest.Uri.ToString());
    }

    [Fact]
    public async Task ApiError_ThrowsPayIslandApiException()
    {
        var body = """{"message":"Invalid request"}""";
        var handler = new RecordingHandler(_ => JsonResponse(body, HttpStatusCode.BadRequest));
        var client = CreateClient(handler);

        var exception = await Assert.ThrowsAsync<PayIslandApiException>(() =>
            client.Transactions.InitializeAsync(new Dictionary<string, object?>()));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        Assert.Equal(body, exception.ResponseBody);
        Assert.NotNull(exception.ResponseData);
        Assert.Equal("Invalid request", GetString(exception.ResponseData, "message"));
    }

    private static PayIslandClient CreateClient(RecordingHandler handler)
    {
        return new PayIslandClient(new PayIslandConfig("test_secret_key"), new HttpClient(handler));
    }

    private static HttpResponseMessage JsonResponse(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    private static string? GetString(Dictionary<string, object?>? data, string key)
    {
        Assert.NotNull(data);
        return data.TryGetValue(key, out var value) && value is JsonElement element && element.ValueKind == JsonValueKind.String
            ? element.GetString()
            : value as string;
    }
}
