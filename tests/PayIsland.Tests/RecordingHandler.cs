namespace PayIsland.Tests;

internal sealed class RecordingHandler : HttpMessageHandler
{
    private readonly Func<RecordedRequest, HttpResponseMessage> _handler;

    public RecordingHandler(Func<RecordedRequest, HttpResponseMessage> handler)
    {
        _handler = handler;
    }

    public RecordedRequest? LastRequest { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);

        LastRequest = new RecordedRequest(
            request.Method,
            request.RequestUri ?? throw new InvalidOperationException("Request URI was missing."),
            body,
            request.Headers.Authorization?.Scheme,
            request.Headers.Authorization?.Parameter,
            request.Content?.Headers.ContentType?.MediaType,
            string.Join(" ", request.Headers.UserAgent.Select(value => value.ToString())));

        return _handler(LastRequest);
    }
}

internal sealed record RecordedRequest(
    HttpMethod Method,
    Uri Uri,
    string? Body,
    string? AuthorizationScheme,
    string? AuthorizationParameter,
    string? ContentType,
    string UserAgent);
