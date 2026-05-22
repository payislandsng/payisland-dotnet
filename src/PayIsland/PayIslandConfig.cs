using System.Diagnostics.CodeAnalysis;

namespace PayIsland;

/// <summary>
/// Configuration for <see cref="PayIslandClient"/>.
/// </summary>
public sealed class PayIslandConfig
{
    /// <summary>
    /// The default PayIsland API base URL.
    /// </summary>
    public const string DefaultBaseUrl = "https://ags.payislands.com";

    /// <summary>
    /// Creates an empty configuration object for object initializer usage.
    /// </summary>
    public PayIslandConfig()
    {
    }

    /// <summary>
    /// Creates configuration with the required PayIsland secret key.
    /// </summary>
    /// <param name="secretKey">PayIsland API secret key.</param>
    [SetsRequiredMembers]
    public PayIslandConfig(string secretKey)
    {
        SecretKey = secretKey;
    }

    /// <summary>
    /// PayIsland API secret key used as a Bearer token.
    /// </summary>
    public required string SecretKey { get; init; }

    /// <summary>
    /// PayIsland API base URL. Defaults to https://ags.payislands.com.
    /// </summary>
    public string BaseUrl { get; init; } = DefaultBaseUrl;

    /// <summary>
    /// Request timeout in seconds. Defaults to 30.
    /// </summary>
    public int TimeoutSeconds { get; init; } = 30;

    internal string NormalizedBaseUrl => BaseUrl.TrimEnd('/');

    internal void Validate()
    {
        if (string.IsNullOrWhiteSpace(SecretKey))
        {
            throw new ArgumentException("PayIsland secret key is required.", nameof(SecretKey));
        }

        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new ArgumentException("PayIsland base URL is required.", nameof(BaseUrl));
        }

        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out _))
        {
            throw new ArgumentException("PayIsland base URL must be an absolute URL.", nameof(BaseUrl));
        }

        if (TimeoutSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(TimeoutSeconds), "TimeoutSeconds must be greater than zero.");
        }
    }
}
