using PayIsland.Support;

namespace PayIsland.Resources;

/// <summary>
/// PayIsland transaction APIs.
/// </summary>
public sealed class TransactionsResource
{
    private const string InitializePath = "/api/v1/transactions/in/initialize";
    private const string VerifyPathPrefix = "/api/v1/transactions/in/check-transaction-status/";

    private readonly PayIslandHttpClient _httpClient;

    internal TransactionsResource(PayIslandHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Initializes an incoming PayIsland transaction.
    /// </summary>
    /// <param name="payload">Transaction initialization payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>PayIsland API response as a JSON-friendly dictionary.</returns>
    public Task<Dictionary<string, object?>> InitializeAsync(
        Dictionary<string, object?> payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return _httpClient.PostAsync(InitializePath, payload, cancellationToken);
    }

    /// <summary>
    /// Verifies an incoming PayIsland transaction by reference.
    /// </summary>
    /// <param name="reference">Transaction reference.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>PayIsland API response as a JSON-friendly dictionary.</returns>
    public Task<Dictionary<string, object?>> VerifyAsync(
        string reference,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reference))
        {
            throw new ArgumentException("Transaction reference is required.", nameof(reference));
        }

        return _httpClient.GetAsync($"{VerifyPathPrefix}{Uri.EscapeDataString(reference)}", cancellationToken);
    }
}
