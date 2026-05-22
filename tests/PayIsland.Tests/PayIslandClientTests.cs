using PayIsland;
using Xunit;

namespace PayIsland.Tests;

public sealed class PayIslandClientTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_RequiresSecretKey(string secretKey)
    {
        var exception = Assert.Throws<ArgumentException>(() => new PayIslandClient(secretKey));
        Assert.Contains("secret key", exception.Message.ToLowerInvariant());
    }

    [Fact]
    public void Constructor_UsesDefaultBaseUrl()
    {
        var client = new PayIslandClient("test_secret_key");
        Assert.Equal("https://ags.payislands.com", client.BaseUrl);
    }

    [Fact]
    public void Constructor_AllowsCustomBaseUrl()
    {
        var client = new PayIslandClient(new PayIslandConfig("test_secret_key")
        {
            BaseUrl = "https://api.example.com/"
        });

        Assert.Equal("https://api.example.com", client.BaseUrl);
    }
}
