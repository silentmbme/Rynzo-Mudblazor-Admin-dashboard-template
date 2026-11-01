namespace Rynzo.Client.Infrastructure.Http;

public sealed class ExpressApiConfiguration
{
    public Uri? BaseAddress { get; private set; }
    public TimeSpan Timeout { get; private set; } = TimeSpan.FromSeconds(30);
    public void Configure(ExpressApiOptions options)
    {
        if (!options.IsValid()) throw new InvalidOperationException("Express API configuration is invalid.");
        BaseAddress = string.IsNullOrWhiteSpace(options.BaseUrl) ? null
            : new Uri(options.BaseUrl.TrimEnd('/') + "/", UriKind.Absolute);
        Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
    }
}
