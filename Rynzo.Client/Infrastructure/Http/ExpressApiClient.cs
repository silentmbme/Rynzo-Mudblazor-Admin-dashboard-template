namespace Rynzo.Client.Infrastructure.Http;
// Typed transport foundation only: no assumed domain routes, DTOs or authentication protocol.
public sealed class ExpressApiClient(HttpClient httpClient, ExpressApiConfiguration configuration)
{
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var baseAddress = configuration.BaseAddress ?? throw new InvalidOperationException(
            "ExpressApi:BaseUrl is not configured. Supply the existing backend URL before using API services.");
        if (request.RequestUri is null || request.RequestUri.IsAbsoluteUri)
            throw new ArgumentException("API requests must specify a relative path.", nameof(request));
        var target = new Uri(baseAddress, request.RequestUri);
        if (!baseAddress.IsBaseOf(target))
            throw new ArgumentException("API requests must remain within the configured base URL.", nameof(request));
        request.RequestUri = target;
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(configuration.Timeout);
        // Buffer within the timeout; caller owns request/response and interprets API error contracts.
        return await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, timeout.Token);
    }
}
