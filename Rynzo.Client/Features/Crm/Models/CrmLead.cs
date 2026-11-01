using System.Globalization;
using System.Text.Json;
namespace Rynzo.Client.Features.Crm.Models;

public sealed record CrmLead
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Role { get; set; } = "";
    public string Company { get; set; } = "";
    public double Score { get; set; }
    public string Stage { get; set; } = "New";
    public string Source { get; set; } = "Website";
    public string Owner { get; set; } = "Sophia Carter";
    public double Value { get; set; }
    public double Probability { get; set; }
    public string Follow { get; set; } = "Aug 30";
    public string FollowKind { get; set; } = "This Week";
    public string Activity { get; set; } = "Lead created";
    public int Days { get; set; }
    public string[] Tags { get; set; } = [];
    public string[] Notes { get; set; } = [];
    public string[] Tasks { get; set; } = [];
    public string Initials { get; set; } = "";
    public string ScoreKind => Score >= 80 ? "Hot" : Score >= 50 ? "Warm" : "Cold";
    public string ValueText => "$" + (Value / 1000).ToString(Value % 1000 != 0 ? "F1" : "F0", CultureInfo.InvariantCulture) + "K";
    public string ProfileValueText => "$" + (Value / 1000).ToString("F1", CultureInfo.InvariantCulture) + "K";
    public string ActivityLabel => Days == 0 ? "2 hours ago" : $"{Days} days ago";
}

public sealed record LeadDraft
{
    public string First { get; set; } = ""; public string Last { get; set; } = ""; public string Email { get; set; } = ""; public string Phone { get; set; } = ""; public string Role { get; set; } = ""; public string Company { get; set; } = "CloudAxis"; public string Stage { get; set; } = "New"; public string Source { get; set; } = "Website"; public string Owner { get; set; } = "Sophia Carter"; public string Score { get; set; } = "50"; public string Value { get; set; } = "0"; public string Probability { get; set; } = "50"; public string Follow { get; set; } = "Aug 30"; public string Tags { get; set; } = ""; public string Notes { get; set; } = "";
}

public sealed record CrmLeadsSource(string[] Owners, string[] Stages, CrmLead[] Leads, Dictionary<string, string> Tones, LeadDraft Blank, string[][] Filters)
{
    public static CrmLeadsSource Load()
    {
        using var stream = typeof(CrmLeadsSource).Assembly.GetManifestResourceStream("Rynzo.CrmLeadsSource")!;
        return JsonSerializer.Deserialize<CrmLeadsSource>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
    }
}
