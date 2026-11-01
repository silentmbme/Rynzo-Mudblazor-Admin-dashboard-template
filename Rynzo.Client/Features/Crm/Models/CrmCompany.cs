using System.Text.Json;
using System.Globalization;
namespace Rynzo.Client.Features.Crm.Models;

public sealed record CrmCompany
{
    public long Id { get; set; }
    public string Name { get; set; } = ""; public string Domain { get; set; } = ""; public string Industry { get; set; } = "SaaS"; public string Size { get; set; } = "51-200"; public double Revenue { get; set; }
    public string Status { get; set; } = "Prospect"; public string Owner { get; set; } = "Sophia Carter"; public string Country { get; set; } = "United States"; public int Contacts { get; set; }
    public int Deals { get; set; }
    public double Pipeline { get; set; }
    public string Activity { get; set; } = "Company created"; public int? Days { get; set; } = 0; public string FollowUp { get; set; } = "Not scheduled"; public string[] Tags { get; set; } = []; public string[] Notes { get; set; } = []; public string[] Tasks { get; set; } = []; public string Initials { get; set; } = "";
    public string ActivityLabel => Days == 0 ? "Today" : Days is not null ? $"{Days} days ago" : "No activity";
    public string PipelineValue => "$" + (Pipeline / 1000).ToString("F0", CultureInfo.InvariantCulture) + "K";
    public string RevenueValue => "$" + Math.Floor(Revenue / 1e6 + .5).ToString("0", CultureInfo.InvariantCulture) + "M";
    public string OwnerInitials => string.Concat(Owner.Split(' ').Select(item => item.Length > 0 ? item[..1] : ""));
    public int SizeStart => int.TryParse(Size.Split('-', '+')[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : 0;
}
public sealed record CompanyDraft { public string Name { get; set; } = ""; public string Domain { get; set; } = ""; public string Industry { get; set; } = "SaaS"; public string Size { get; set; } = "51-200"; public string Revenue { get; set; } = "0"; public string Status { get; set; } = "Prospect"; public string Owner { get; set; } = "Sophia Carter"; public string Country { get; set; } = "United States"; public string Tags { get; set; } = ""; public string Notes { get; set; } = ""; }
public sealed record CrmCompaniesSource(string[] Owners, CrmCompany[] Companies, Dictionary<string, string> Tones, CompanyDraft Blank, Dictionary<string, string[]> FormOptions, string[][] Filters)
{
    public static CrmCompaniesSource Load() { using var stream = typeof(CrmCompaniesSource).Assembly.GetManifestResourceStream("Rynzo.CrmCompaniesSource")!; return JsonSerializer.Deserialize<CrmCompaniesSource>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web))!; }
}
