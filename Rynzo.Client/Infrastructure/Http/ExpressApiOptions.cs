namespace Rynzo.Client.Infrastructure.Http;
// Only public non-secret settings cross the server/client render boundary.
public sealed class ExpressApiOptions
{
    public const string SectionName = "ExpressApi";
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public bool IsValid() => TimeoutSeconds is >= 1 and <= 300 &&
        (string.IsNullOrWhiteSpace(BaseUrl) ||
         (Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri) &&
          uri.Scheme is "http" or "https" && string.IsNullOrEmpty(uri.UserInfo) &&
          string.IsNullOrEmpty(uri.Query) && string.IsNullOrEmpty(uri.Fragment)));
}
