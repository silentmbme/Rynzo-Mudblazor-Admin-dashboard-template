namespace Rynzo.Client.Features.Help.Models;

// Local dialog fields only; no delivery or API contract exists.
public sealed class SupportDraft
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Topic { get; set; } = "Account";
    public string Subject { get; set; } = "";
    public string Message { get; set; } = "";
}
