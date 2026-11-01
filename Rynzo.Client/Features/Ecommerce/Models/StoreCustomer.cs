using System.Text.Json;
using System.Globalization;
namespace Rynzo.Client.Features.Ecommerce.Models;

public sealed record StoreCustomer
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Company { get; set; } = "";
    public string Initials { get; set; } = "";
    public string City { get; set; } = "";
    public string Country { get; set; } = "";
    public int Orders { get; set; }
    public decimal Spent { get; set; }
    public string LastOrder { get; set; } = "Never";
    public int? LastOrderDays { get; set; }
    public string Segment { get; set; } = "New";
    public string Status { get; set; } = "Active";
    public string Joined { get; set; } = "Aug 24, 2026";
    public string[] Tags { get; set; } = [];
    public string[] Notes { get; set; } = [];
    public string Total => "$" + Spent.ToString("#,0.###", CultureInfo.GetCultureInfo("en-US"));
    public string Average => "$" + (Orders > 0 ? Spent / Orders : 0).ToString("F2", CultureInfo.InvariantCulture);
    public string Lifetime => "$" + Math.Floor((double)Spent * 1.3 + .5).ToString("N0", CultureInfo.GetCultureInfo("en-US"));
    public string RecentAmount => "$" + Math.Max(89, Math.Floor((double)Spent / Math.Max(1, Orders) + .5)).ToString("F2", CultureInfo.InvariantCulture);
}
public sealed record CustomerDraft
{
    public string First { get; set; } = ""; public string Last { get; set; } = ""; public string Email { get; set; } = ""; public string Phone { get; set; } = ""; public string Company { get; set; } = ""; public string Country { get; set; } = "United States"; public string City { get; set; } = ""; public string Segment { get; set; } = "New"; public string Status { get; set; } = "Active"; public string Tags { get; set; } = ""; public string Notes { get; set; } = "";
}
public sealed record CustomersSource(StoreCustomer[] Customers, string[] Segments, string[] Statuses, Dictionary<string, string> StatusTone, Dictionary<string, string> SegmentTone, CustomerDraft EmptyForm, string[][] Filters)
{
    public static CustomersSource Load() { using var stream = typeof(CustomersSource).Assembly.GetManifestResourceStream("Rynzo.CustomersSource")!; return JsonSerializer.Deserialize<CustomersSource>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web))!; }
}
