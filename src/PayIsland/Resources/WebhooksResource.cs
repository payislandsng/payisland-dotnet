using System.Security.Cryptography;
using System.Text;

namespace PayIsland.Resources;

/// <summary>
/// PayIsland webhook verification helpers.
/// </summary>
public sealed class WebhooksResource
{
    /// <summary>
    /// Verifies a PayIsland webhook signature using an UTF-8 string payload.
    /// </summary>
    /// <param name="payload">Raw webhook payload.</param>
    /// <param name="signature">Webhook signature.</param>
    /// <param name="secret">Webhook secret.</param>
    /// <returns>True when the signature matches; otherwise false.</returns>
    public bool VerifySignature(string payload, string signature, string secret)
    {
        if (payload is null)
        {
            return false;
        }

        return VerifySignature(Encoding.UTF8.GetBytes(payload), signature, secret);
    }

    /// <summary>
    /// Verifies a PayIsland webhook signature using raw payload bytes.
    /// </summary>
    /// <param name="payload">Raw webhook payload bytes.</param>
    /// <param name="signature">Webhook signature.</param>
    /// <param name="secret">Webhook secret.</param>
    /// <returns>True when the signature matches; otherwise false.</returns>
    public bool VerifySignature(byte[] payload, string signature, string secret)
    {
        if (payload is null || string.IsNullOrWhiteSpace(signature) || string.IsNullOrWhiteSpace(secret))
        {
            return false;
        }

        var providedSignature = DecodeSignature(signature);
        if (providedSignature is null)
        {
            return false;
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var expectedSignature = hmac.ComputeHash(payload);

        return CryptographicOperations.FixedTimeEquals(expectedSignature, providedSignature);
    }

    private static byte[]? DecodeSignature(string signature)
    {
        var normalized = signature.Trim();

        const string Sha256Prefix = "sha256=";
        if (normalized.StartsWith(Sha256Prefix, StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[Sha256Prefix.Length..];
        }

        var hexBytes = DecodeHex(normalized);
        if (hexBytes is not null)
        {
            return hexBytes;
        }

        try
        {
            return Convert.FromBase64String(normalized);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private static byte[]? DecodeHex(string value)
    {
        if (value.Length == 0 || value.Length % 2 != 0)
        {
            return null;
        }

        var bytes = new byte[value.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var high = FromHex(value[i * 2]);
            var low = FromHex(value[(i * 2) + 1]);

            if (high < 0 || low < 0)
            {
                return null;
            }

            bytes[i] = (byte)((high << 4) | low);
        }

        return bytes;
    }

    private static int FromHex(char character)
    {
        return character switch
        {
            >= '0' and <= '9' => character - '0',
            >= 'a' and <= 'f' => character - 'a' + 10,
            >= 'A' and <= 'F' => character - 'A' + 10,
            _ => -1
        };
    }
}
