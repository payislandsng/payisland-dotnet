using System.Text.Json;
using PayIsland;
using PayIsland.Exceptions;

namespace PayIsland.Examples;

public static class VerifyPaymentExample
{
    public static async Task RunAsync(string[] args)
    {
        var secretKey = Environment.GetEnvironmentVariable("PAYISLAND_SECRET_KEY");

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            Console.WriteLine("Missing PAYISLAND_SECRET_KEY. Add it to your environment and try again.");
            return;
        }

        if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
        {
            Console.WriteLine("Usage: dotnet run --project examples/PayIsland.Examples -- verify <transaction_reference>");
            return;
        }

        var payIsland = new PayIslandClient(secretKey);

        try
        {
            var response = await payIsland.Transactions.VerifyAsync(args[0]);
            Console.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (PayIslandApiException exception)
        {
            Console.WriteLine($"PayIsland API error: {(int)exception.StatusCode} {exception.StatusCode}");
            Console.WriteLine(exception.ResponseBody);
        }
    }
}
