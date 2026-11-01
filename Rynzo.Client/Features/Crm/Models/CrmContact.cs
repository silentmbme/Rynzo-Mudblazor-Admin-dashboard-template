using System.Text.Json;
using System.Globalization;
namespace Rynzo.Client.Features.Crm.Models;

public sealed record CrmContact
{
    public long Id { get; set; }
    public string Name { get; set; } = ""; public string Email { get; set; } = ""; public string Phone { get; set; } = ""; public string Role { get; set; } = ""; public string Company { get; set; } = ""; public string Industry { get; set; } = "Technology"; public string Owner { get; set; } = "Sophia Carter"; public string Source { get; set; } = "Website"; public string Type { get; set; } = "Lead"; public string Status { get; set; } = "New"; public string[] Tags { get; set; } = []; public double Score { get; set; }
    public double Pipeline { get; set; }
    public string Activity { get; set; } = "Profile created"; public int? ActivityDays { get; set; } = 0; public string FollowUp { get; set; } = "Aug 30"; public bool Overdue { get; set; }
    public string[] Notes { get; set; } = []; public string[] Tasks { get; set; } = []; public string Initials { get; set; } = ""; public string Location { get; set; } = "New York, US";
    public string ActivityLabel => ActivityDays == 0 ? "Today" : ActivityDays is not null ? $"{ActivityDays} days ago" : "No activity";
    public string DealValue => "$" + (Pipeline / 1000).ToString(Pipeline % 1000 != 0 ? "F1" : "F0", CultureInfo.InvariantCulture) + "K";
    public string ProfileValue => "$" + (Pipeline / 1000).ToString("F1", CultureInfo.InvariantCulture) + "K";
    public string OwnerInitials => string.Concat(Owner.Split(' ').Select(part => part.Length > 0 ? part[..1] : ""));
}
public sealed record ContactDraft { public string First { get; set; } = ""; public string Last { get; set; } = ""; public string Email { get; set; } = ""; public string Phone { get; set; } = ""; public string Role { get; set; } = ""; public string Company { get; set; } = "CloudAxis"; public string Type { get; set; } = "Lead"; public string Status { get; set; } = "New"; public string Owner { get; set; } = "Sophia Carter"; public string Source { get; set; } = "Website"; public string Score { get; set; } = "50"; public string Tags { get; set; } = ""; public string FollowUp { get; set; } = "Aug 30"; public string Notes { get; set; } = ""; }
public sealed record CrmContactsSource(string[] Owners, CrmContact[] Contacts, Dictionary<string, string> StatusTone, ContactDraft Blank, string[][] Filters)
{
    public static CrmContactsSource Load() { using var stream = typeof(CrmContactsSource).Assembly.GetManifestResourceStream("Rynzo.CrmContactsSource")!; return JsonSerializer.Deserialize<CrmContactsSource>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web))!; }
}
