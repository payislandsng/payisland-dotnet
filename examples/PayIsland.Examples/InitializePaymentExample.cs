using System.Text.Json;
using PayIsland;
using PayIsland.Exceptions;

namespace PayIsland.Examples;

public static class InitializePaymentExample
{
    public static async Task Main(string[] args)
    {
        if (args.Length > 0 && string.Equals(args[0], "verify", StringComparison.OrdinalIgnoreCase))
        {
            await VerifyPaymentExample.RunAsync(args[1..]);
            return;
        }

        if (args.Length > 0 && string.Equals(args[0], "webhook", StringComparison.OrdinalIgnoreCase))
        {
            WebhookVerificationExample.Run();
            return;
        }

        var secretKey = Environment.GetEnvironmentVariable("PAYISLAND_SECRET_KEY");
        var paymentItemId = Environment.GetEnvironmentVariable("PAYISLAND_PAYMENT_ITEM_ID");

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            Console.WriteLine("Missing PAYISLAND_SECRET_KEY. Add it to your environment and try again.");
            return;
        }

        if (string.IsNullOrWhiteSpace(paymentItemId))
        {
            Console.WriteLine("Missing PAYISLAND_PAYMENT_ITEM_ID. Add it to your environment and try again.");
            return;
        }

        var payIsland = new PayIslandClient(secretKey);

        try
        {
            var response = await payIsland.Transactions.InitializeAsync(new Dictionary<string, object?>
            {
                ["callback_url"] = "https://example.com/webhooks/payislands",
                ["payment_item_id"] = paymentItemId,
                ["transaction_reference"] = $"order_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}",
                ["channel"] = "card",
                ["amount"] = "1000",
                ["customer_info"] = new Dictionary<string, object?>
                {
                    ["email"] = "ada@example.com",
                    ["phone_number"] = "08011112222",
                    ["first_name"] = "Ada",
                    ["last_name"] = "Lovelace"
                }
            });

            var authorizationUrl = TryGetString(response, "authorization_url")
                ?? TryGetNestedString(response, "data", "authorization_url");

            if (!string.IsNullOrWhiteSpace(authorizationUrl))
            {
                Console.WriteLine($"Authorization URL: {authorizationUrl}");
                return;
            }

            Console.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (PayIslandApiException exception)
        {
            Console.WriteLine($"PayIsland API error: {(int)exception.StatusCode} {exception.StatusCode}");
            Console.WriteLine(exception.ResponseBody);
        }
    }

    private static string? TryGetString(Dictionary<string, object?> response, string key)
    {
        return response.TryGetValue(key, out var value) && value is JsonElement element && element.ValueKind == JsonValueKind.String
            ? element.GetString()
            : value as string;
    }

    private static string? TryGetNestedString(Dictionary<string, object?> response, string parentKey, string childKey)
    {
        if (!response.TryGetValue(parentKey, out var value) || value is not JsonElement element || element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        return element.TryGetProperty(childKey, out var child) && child.ValueKind == JsonValueKind.String
            ? child.GetString()
            : null;
    }
}
