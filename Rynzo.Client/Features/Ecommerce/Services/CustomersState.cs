using Rynzo.Client.Features.Ecommerce.Models;
using System.Text.RegularExpressions;
namespace Rynzo.Client.Features.Ecommerce.Services;

public sealed class CustomersState
{
    public CustomersSource Source { get; } = CustomersSource.Load();
    public List<StoreCustomer> Customers { get; }
    public List<long> Selected { get; } = [];
    public StoreCustomer? Active { get; set; }
    public string Query { get; set; } = "";
    public string[] Filters { get; } = ["All Customers", "All statuses", "All regions", "All spend", "All orders", "Any time", "Newest"];
    public int PageSize { get; set; } = 20;
    public string Note { get; set; } = "";
    public string BulkValue { get; set; } = "VIP";
    public CustomerDraft Draft { get; set; } = new();
    public CustomersState() { Customers = [.. Source.Customers]; Draft = Source.EmptyForm with { }; }
    public StoreCustomer[] Filtered
    {
        get { var rows = Customers.Where(c => string.Join(' ', new[] { c.Name, c.Email, c.Phone, c.Company, c.City, c.Country }.Concat(c.Tags)).Contains(Query, StringComparison.OrdinalIgnoreCase) && (Filters[0] == "All Customers" || c.Segment == Filters[0]) && (Filters[1] == "All statuses" || c.Status == Filters[1]) && (Filters[2] == "All regions" || c.Country == Filters[2]) && (Filters[3] switch { "Under $500" => c.Spent < 500, "$500-$1,000" => c.Spent >= 500 && c.Spent <= 1000, "$1,000-$5,000" => c.Spent > 1000 && c.Spent <= 5000, "$5,000+" => c.Spent > 5000, _ => true }) && (Filters[4] switch { "0 Orders" => c.Orders == 0, "1-5" => c.Orders >= 1 && c.Orders <= 5, "6-10" => c.Orders >= 6 && c.Orders <= 10, "10+" => c.Orders > 10, _ => true }) && (Filters[5] switch { "Never Ordered" => c.LastOrderDays is null, "Last 7 Days" => (c.LastOrderDays ?? 999) <= 7, "Last 30 Days" => (c.LastOrderDays ?? 999) <= 30, "Last 90 Days" => (c.LastOrderDays ?? 999) <= 90, "More Than 90 Days" => (c.LastOrderDays ?? 0) > 90, _ => true })); return (Filters[6] switch { "Name A-Z" => rows.OrderBy(c => c.Name, StringComparer.Create(System.Globalization.CultureInfo.GetCultureInfo("en-US"), false)), "Highest Spend" or "Highest Customer Value" => rows.OrderByDescending(c => c.Spent), "Lowest Spend" => rows.OrderBy(c => c.Spent), "Most Orders" => rows.OrderByDescending(c => c.Orders), "Recent Purchase" => rows.OrderBy(c => c.LastOrderDays ?? 999), "Oldest" => rows.OrderBy(c => c.Id), _ => rows.OrderByDescending(c => c.Id) }).ToArray(); }
    }
    public StoreCustomer[] Visible => Filtered.Take(PageSize).ToArray();
    public bool AllSelected => Visible.Length > 0 && Visible.All(c => Selected.Contains(c.Id));
    public void Reset() { Query = ""; for (var i = 0; i < Filters.Length; i++) Filters[i] = Source.Filters[i][0]; }
    public void Toggle(long id) { if (!Selected.Remove(id)) Selected.Add(id); }
    public void SelectAll() { var clear = AllSelected; Selected.Clear(); if (!clear) Selected.AddRange(Visible.Select(c => c.Id)); }
    public void BeginAdd() => Draft = Source.EmptyForm with { };
    public void BeginEdit(StoreCustomer c) { Active = c; var names = c.Name.Split(' '); Draft = new() { First = names[0], Last = string.Join(' ', names.Skip(1)), Email = c.Email, Phone = c.Phone, Company = c.Company, Country = c.Country, City = c.City, Segment = c.Segment, Status = c.Status, Tags = string.Join(", ", c.Tags), Notes = string.Join(' ', c.Notes) }; }
    public bool Save(bool add) { if (string.IsNullOrWhiteSpace(Draft.First) || !Regex.IsMatch(Draft.Email, @"^\S+@\S+\.\S+$")) return false; StoreCustomer Patch(StoreCustomer c) => c with { Name = $"{Draft.First} {Draft.Last}".Trim(), Email = Draft.Email, Phone = Draft.Phone, Company = Draft.Company, Initials = ((Draft.First.Length > 0 ? Draft.First[..1] : "") + (Draft.Last.Length > 0 ? Draft.Last[..1] : "")).ToUpperInvariant(), Country = Draft.Country, City = Draft.City, Segment = Draft.Segment, Status = Draft.Status, Tags = Draft.Tags.Split(',').Select(t => t.Trim()).Where(t => t.Length > 0).ToArray(), Notes = Draft.Notes.Length > 0 ? [Draft.Notes] : [] }; if (add) Customers.Add(Patch(new() { Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() })); else if (Active is not null) Update(Active.Id, Patch); return true; }
    public void Update(long id, Func<StoreCustomer, StoreCustomer> patch) { for (var i = 0; i < Customers.Count; i++) if (Customers[i].Id == id) Customers[i] = patch(Customers[i]); if (Active?.Id == id) Active = patch(Active); }
    public void Bulk(Func<StoreCustomer, StoreCustomer> patch) { for (var i = 0; i < Customers.Count; i++) if (Selected.Contains(Customers[i].Id)) Customers[i] = patch(Customers[i]); }
    public void Confirm(string mode) { if (mode == "bulkSegment") Bulk(c => c with { Segment = BulkValue }); else if (mode == "bulkStatus") Bulk(c => c with { Status = BulkValue }); else if (mode == "bulkDelete") { Customers.RemoveAll(c => Selected.Contains(c.Id)); Selected.Clear(); } else if (Active is not null) { var id = Active.Id; if (mode == "block") Update(id, c => c with { Status = c.Status == "Blocked" ? "Active" : "Blocked" }); if (mode == "archive") Update(id, c => c with { Status = "Archived" }); if (mode == "delete") { Customers.RemoveAll(c => c.Id == id); Active = null; } if (mode == "note" && Note.Trim().Length > 0) Update(id, c => c with { Notes = [.. c.Notes, Note.Trim()] }); } Note = ""; }
    public string Export(bool selected = false) { var rows = selected ? Customers.Where(c => Selected.Contains(c.Id)) : Filtered; return "Name,Email,Location,Orders,Total spent,Segment,Status" + string.Concat(rows.Select(c => "\n" + string.Join(',', c.Name, c.Email, $"{c.City} {c.Country}", c.Orders.ToString(System.Globalization.CultureInfo.InvariantCulture), c.Spent.ToString("0.############################", System.Globalization.CultureInfo.InvariantCulture), c.Segment, c.Status))); }
}
