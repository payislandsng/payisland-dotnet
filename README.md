# PayIsland .NET SDK

Official .NET SDK for integrating with PayIsland payment APIs.

## Installation

```bash
dotnet add package PayIsland
```

## Initialization

```csharp
using PayIsland;

var payIsland = new PayIslandClient("test_secret_key");
```

PayIsland uses your API key to determine sandbox or live mode. The SDK does not expose a separate environment flag.

## Transaction Initialization

```csharp
using PayIsland;

var payIsland = new PayIslandClient("test_secret_key");

var response = await payIsland.Transactions.InitializeAsync(new Dictionary<string, object?>
{
    ["callback_url"] = "https://example.com/webhooks/payislands",
    ["payment_item_id"] = "6",
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

Console.WriteLine(response);
```

For card transactions, PayIsland may return a pending state while 3DS or another customer authorization step is in progress. Redirect the customer to the authorization URL when one is returned, then verify the transaction reference before fulfillment.

## Transaction Verification

```csharp
var response = await payIsland.Transactions.VerifyAsync("order_123");
Console.WriteLine(response);
```

This sends:

```http
GET /api/v1/transactions/in/check-transaction-status/{reference}
```

## Webhook Verification

```csharp
var isValid = payIsland.Webhooks.VerifySignature(
    rawPayload,
    signature,
    webhookSecret
);
```

Always verify the webhook signature and verify the transaction reference with PayIsland before fulfillment.

## Error Handling

The SDK throws `PayIslandApiException` for non-success API responses.

```csharp
using PayIsland.Exceptions;

try
{
    var response = await payIsland.Transactions.VerifyAsync("order_123");
}
catch (PayIslandApiException exception)
{
    Console.WriteLine(exception.StatusCode);
    Console.WriteLine(exception.ResponseBody);
}
```

## Configuration

```csharp
var payIsland = new PayIslandClient(new PayIslandConfig("test_secret_key")
{
    BaseUrl = "https://ags.payislands.com",
    TimeoutSeconds = 30
});
```

Default headers sent by the SDK:

- `Authorization: Bearer <secretKey>`
- `Content-Type: application/json`
- `Accept: application/json`
- `User-Agent: payisland-dotnet`

## Examples

Set environment variables from `.env.example`, then run:

```bash
dotnet run --project examples/PayIsland.Examples
dotnet run --project examples/PayIsland.Examples -- verify <transaction_reference>
dotnet run --project examples/PayIsland.Examples -- webhook
```

## Development Commands

```bash
dotnet restore
dotnet build
dotnet test
dotnet pack -c Release
```

## NuGet Publishing

```bash
dotnet pack -c Release
dotnet nuget push src/PayIsland/bin/Release/PayIsland.0.1.0.nupkg \
  --api-key <NUGET_API_KEY> \
  --source https://api.nuget.org/v3/index.json
```

## License

MIT
