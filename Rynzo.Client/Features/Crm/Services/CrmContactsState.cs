using Rynzo.Client.Features.Crm.Models;
using System.Text.RegularExpressions;
using System.Globalization;
namespace Rynzo.Client.Features.Crm.Services;

public sealed class CrmContactsState
{
    public CrmContactsSource Data { get; } = CrmContactsSource.Load();
    public List<CrmContact> Contacts { get; }
    public List<long> Selected { get; } = [];
    public CrmContact? Active { get; set; }
    public ContactDraft Draft { get; set; } = new();
    public string Query { get; set; } = "";
    public string[] Filters { get; } = ["All types", "All statuses", "All companies", "All owners", "All sources", "All tags", "Any activity", "Recently Added"];
    public string Note { get; set; } = ""; public string TaskTitle { get; set; } = ""; public string BulkValue { get; set; } = "Sophia Carter"; public int PageSize { get; set; } = 20;
    public CrmContactsState() { Contacts = [.. Data.Contacts]; Draft = Data.Blank with { }; }
    public CrmContact[] Filtered { get { var rows = Contacts.Where(c => string.Join(' ', new[] { c.Name, c.Email, c.Phone, c.Company, c.Role }.Concat(c.Tags)).Contains(Query, StringComparison.OrdinalIgnoreCase) && (Filters[0] == "All types" || c.Type == Filters[0]) && (Filters[1] == "All statuses" || c.Status == Filters[1]) && (Filters[2] == "All companies" || c.Company == Filters[2]) && (Filters[3] == "All owners" || c.Owner == Filters[3]) && (Filters[4] == "All sources" || c.Source == Filters[4]) && (Filters[5] == "All tags" || c.Tags.Contains(Filters[5])) && (Filters[6] switch { "Today" => (c.ActivityDays ?? 99) == 0, "Last 7 Days" => (c.ActivityDays ?? 99) <= 7, "Last 30 Days" => (c.ActivityDays ?? 99) <= 30, "No Activity" => c.ActivityDays is null, "Overdue Follow-up" => c.Overdue, _ => true })); var comparer = StringComparer.Create(CultureInfo.GetCultureInfo("en-US"), false); return (Filters[7] switch { "Name A-Z" => rows.OrderBy(c => c.Name, comparer), "Highest Deal Value" => rows.OrderByDescending(c => c.Pipeline), "Last Activity" => rows.OrderBy(c => c.ActivityDays ?? 99), "Next Follow-up" => rows.OrderBy(c => c.FollowUp, comparer), "Company A-Z" => rows.OrderBy(c => c.Company, comparer), _ => rows.OrderByDescending(c => c.Id) }).ToArray(); } }
    public CrmContact[] Visible => Filtered.Take(PageSize).ToArray();
    public bool AllSelected => Visible.Length > 0 && Visible.All(c => Selected.Contains(c.Id));
    public void Reset() { Query = ""; for (var i = 0; i < Filters.Length; i++) Filters[i] = Data.Filters[i][0]; }
    public void Toggle(long id) { if (!Selected.Remove(id)) Selected.Add(id); }
    public void SelectAll() { var clear = AllSelected; Selected.Clear(); if (!clear) Selected.AddRange(Visible.Select(c => c.Id)); }
    public void BeginAdd() => Draft = Data.Blank with { };
    public void BeginEdit(CrmContact contact) { Active = contact; var name = contact.Name.Split(' '); Draft = new() { First = name[0], Last = string.Join(' ', name.Skip(1)), Email = contact.Email, Phone = contact.Phone, Role = contact.Role, Company = contact.Company, Type = contact.Type, Status = contact.Status, Owner = contact.Owner, Source = contact.Source, Score = contact.Score.ToString(CultureInfo.InvariantCulture), Tags = string.Join(", ", contact.Tags), FollowUp = contact.FollowUp, Notes = string.Join(' ', contact.Notes) }; }
    public void Update(long id, Func<CrmContact, CrmContact> patch) { for (var i = 0; i < Contacts.Count; i++) if (Contacts[i].Id == id) Contacts[i] = patch(Contacts[i]); if (Active?.Id == id) Active = patch(Active); }
    public void Bulk(Func<CrmContact, CrmContact> patch) { for (var i = 0; i < Contacts.Count; i++) if (Selected.Contains(Contacts[i].Id)) Contacts[i] = patch(Contacts[i]); }
    private static double Score(string value) => Rynzo.Client.Utilities.JavaScriptNumber.OrZero(value);
    public bool Save(bool add) { if (Draft.First.Length == 0 || Draft.Last.Length == 0 || Draft.Email.Length > 0 && !Regex.IsMatch(Draft.Email, @"^\S+@\S+\.\S+$")) return false; CrmContact Patch(CrmContact c) => c with { Name = $"{Draft.First} {Draft.Last}", Email = Draft.Email, Phone = Draft.Phone, Role = Draft.Role, Company = Draft.Company, Type = Draft.Type, Status = Draft.Status, Owner = Draft.Owner, Source = Draft.Source, Score = Score(Draft.Score), Tags = Draft.Tags.Split(',').Select(t => t.Trim()).Where(t => t.Length > 0).ToArray(), FollowUp = Draft.FollowUp, Notes = Draft.Notes.Length > 0 ? [Draft.Notes] : [] }; if (add) Contacts.Add(Patch(new() { Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), Initials = (Draft.First[..1] + Draft.Last[..1]).ToUpperInvariant() })); else if (Active is not null) Update(Active.Id, Patch); return true; }
    public bool Confirm(string mode) { if (mode == "bulkOwner") Bulk(c => c with { Owner = BulkValue }); else if (mode == "bulkStatus") Bulk(c => c with { Status = BulkValue }); else if (mode == "bulkTag") Bulk(c => c with { Tags = [.. c.Tags, BulkValue] }); else if (mode == "bulkDelete") { Contacts.RemoveAll(c => Selected.Contains(c.Id)); Selected.Clear(); } else if (Active is not null) { var id = Active.Id; if (mode == "note" && Note.Trim().Length > 0) Update(id, c => c with { Notes = [.. c.Notes, Note] }); else if (mode == "task" && TaskTitle.Trim().Length > 0) Update(id, c => c with { Tasks = [.. c.Tasks, TaskTitle] }); else if (mode == "owner") Update(id, c => c with { Owner = BulkValue }); else if (mode == "archive") Update(id, c => c with { Status = "Archived" }); else if (mode == "delete") { Contacts.RemoveAll(c => c.Id == id); Active = null; } else return false; } else return false; Note = ""; TaskTitle = ""; return true; }
    public string Export(bool selected = false) { var rows = selected ? Contacts.Where(c => Selected.Contains(c.Id)) : Filtered; return "Name,Email,Company,Owner,Status,Pipeline" + string.Concat(rows.Select(c => "\n" + string.Join(',', c.Name, c.Email, c.Company, c.Owner, c.Status, c.Pipeline.ToString(CultureInfo.InvariantCulture)))); }
}
