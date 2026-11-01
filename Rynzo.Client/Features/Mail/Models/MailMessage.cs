using System.Text.Json;
namespace Rynzo.Client.Features.Mail.Models;

public sealed record MailMessage(int Index, string Sender, string Subject, string Preview, string Time, string Initials)
{
    public static MailMessage[] Load() { using var stream = typeof(MailMessage).Assembly.GetManifestResourceStream("Rynzo.MailSource") ?? throw new InvalidOperationException("Mail source unavailable."); return JsonSerializer.Deserialize<MailMessage[]>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Mail source invalid."); }
    public string Address => Sender.ToLowerInvariant().Replace(" ", ".", StringComparison.Ordinal) + "@rynzo.example";
    public string Portrait => Initials == "SC" ? "assets/images/users/sophia-carter.jpg" : Initials == "DL" ? "assets/images/users/user-4.jpg" : "assets/images/users/user-1.jpg";
}
