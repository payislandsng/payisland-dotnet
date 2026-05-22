using System.Security.Cryptography;
using System.Text;
using PayIsland;

namespace PayIsland.Examples;

public static class WebhookVerificationExample
{
    public static void Run()
    {
        var rawPayload = """{"transaction_reference":"order_123","status":"success"}""";
        var webhookSecret = Environment.GetEnvironmentVariable("PAYISLAND_WEBHOOK_SECRET") ?? "example_webhook_secret";
        var signature = CreateExampleSignature(rawPayload, webhookSecret);

        var payIsland = new PayIslandClient("test_secret_key");
        var isValid = payIsland.Webhooks.VerifySignature(rawPayload, signature, webhookSecret);

        Console.WriteLine(isValid ? "Webhook signature is valid." : "Webhook signature is invalid.");

        // Merchants should verify the webhook signature and verify the transaction reference before fulfillment.
    }

    private static string CreateExampleSignature(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
    }
}
