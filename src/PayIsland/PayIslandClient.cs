using PayIsland.Resources;
using PayIsland.Support;

namespace PayIsland;

/// <summary>
/// Entry point for the PayIsland .NET SDK.
/// </summary>
public sealed class PayIslandClient
{
    private readonly PayIslandHttpClient _httpClient;

    /// <summary>
    /// Creates a PayIsland client with a secret key and the default API base URL.
    /// </summary>
    /// <param name="secretKey">PayIsland API secret key.</param>
    public PayIslandClient(string secretKey)
        : this(new PayIslandConfig(secretKey))
    {
    }

    /// <summary>
    /// Creates a PayIsland client with a secret key and injected <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="secretKey">PayIsland API secret key.</param>
    /// <param name="httpClient">HTTP client to use for requests.</param>
    public PayIslandClient(string secretKey, HttpClient httpClient)
        : this(new PayIslandConfig(secretKey), httpClient)
    {
    }

    /// <summary>
    /// Creates a PayIsland client with explicit configuration.
    /// </summary>
    /// <param name="config">PayIsland configuration.</param>
    public PayIslandClient(PayIslandConfig config)
        : this(config, new HttpClient())
    {
    }

    /// <summary>
    /// Creates a PayIsland client with explicit configuration and injected <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="config">PayIsland configuration.</param>
    /// <param name="httpClient">HTTP client to use for requests.</param>
    public PayIslandClient(PayIslandConfig config, HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(httpClient);

        config.Validate();

        BaseUrl = config.NormalizedBaseUrl;
        _httpClient = new PayIslandHttpClient(config, httpClient);
        Transactions = new TransactionsResource(_httpClient);
        Webhooks = new WebhooksResource();
    }

    /// <summary>
    /// Gets the configured API base URL.
    /// </summary>
    public string BaseUrl { get; }

    /// <summary>
    /// Access transaction initialization and verification APIs.
    /// </summary>
    public TransactionsResource Transactions { get; }

    /// <summary>
    /// Access webhook signature verification helpers.
    /// </summary>
    public WebhooksResource Webhooks { get; }
}
