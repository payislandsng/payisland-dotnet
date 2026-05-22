using System.Net;

namespace PayIsland.Exceptions;

/// <summary>
/// Exception thrown when PayIsland returns a non-success API response.
/// </summary>
public sealed class PayIslandApiException : Exception
{
    /// <summary>
    /// Creates a PayIsland API exception.
    /// </summary>
    /// <param name="statusCode">HTTP status code returned by PayIsland.</param>
    /// <param name="responseBody">Raw response body returned by PayIsland.</param>
    /// <param name="responseData">Parsed JSON response data, when available.</param>
    public PayIslandApiException(
        HttpStatusCode statusCode,
        string? responseBody,
        Dictionary<string, object?>? responseData = null)
        : base($"PayIsland API request failed with status code {(int)statusCode} ({statusCode}).")
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
        ResponseData = responseData;
    }

    /// <summary>
    /// HTTP status code returned by PayIsland.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Raw response body returned by PayIsland.
    /// </summary>
    public string? ResponseBody { get; }

    /// <summary>
    /// Parsed JSON response data, when available.
    /// </summary>
    public Dictionary<string, object?>? ResponseData { get; }
}
