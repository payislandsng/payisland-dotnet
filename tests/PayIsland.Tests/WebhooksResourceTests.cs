using System.Security.Cryptography;
using System.Text;
using PayIsland;
using Xunit;

namespace PayIsland.Tests;

public sealed class WebhooksResourceTests
{
    private readonly PayIslandClient _client = new("test_secret_key");

    [Fact]
    public void VerifySignature_ReturnsTrueForValidHexSignature()
    {
        var payload = """{"transaction_reference":"order_123"}""";
        var secret = "webhook_secret";
        var signature = Sign(payload, secret);

        Assert.True(_client.Webhooks.VerifySignature(payload, signature, secret));
    }

    [Fact]
    public void VerifySignature_ReturnsTrueForValidPrefixedHexSignature()
    {
        var payload = """{"transaction_reference":"order_123"}""";
        var secret = "webhook_secret";
        var signature = $"sha256={Sign(payload, secret)}";

        Assert.True(_client.Webhooks.VerifySignature(Encoding.UTF8.GetBytes(payload), signature, secret));
    }

    [Fact]
    public void VerifySignature_ReturnsFalseForInvalidSignature()
    {
        Assert.False(_client.Webhooks.VerifySignature("{}", new string('a', 64), "webhook_secret"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-a-valid-signature")]
    public void VerifySignature_ReturnsFalseForMismatchedOrEmptySignature(string signature)
    {
        Assert.False(_client.Webhooks.VerifySignature("{}", signature, "webhook_secret"));
    }

    private static string Sign(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
    }
}
