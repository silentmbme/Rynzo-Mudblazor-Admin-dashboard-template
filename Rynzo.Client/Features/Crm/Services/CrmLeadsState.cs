using Rynzo.Client.Features.Crm.Models;
using Rynzo.Client.Utilities;
using System.Globalization;
using System.Text.RegularExpressions;
namespace Rynzo.Client.Features.Crm.Services;

public sealed class CrmLeadsState
{
    public CrmLeadsSource Data { get; } = CrmLeadsSource.Load();
    public List<CrmLead> Leads { get; }
    public List<long> Selected { get; } = [];
    public CrmLead? Active { get; set; }
    public LeadDraft Draft { get; set; } = new();
    public string Query { get; set; } = "";
    public string[] Filters { get; }
    public string Text { get; set; } = "";
    public string BulkValue { get; set; } = "Sophia Carter";
    public int PageSize { get; set; } = 20;
    public CrmLeadsState() { Leads = [.. Data.Leads]; Draft = Data.Blank with { }; Filters = Data.Filters.Select(x => x[0]).ToArray(); }
    public CrmLead[] Filtered
    {
        get
        {
            var rows = Leads.Where(l => string.Join(' ', new[] { l.Name, l.Email, l.Phone, l.Company, l.Role, l.Source }.Concat(l.Tags)).Contains(Query, StringComparison.OrdinalIgnoreCase)
                && (Filters[0] == "All stages" || l.Stage == Filters[0])
                && (Filters[1] == "All scores" || Filters[1] == "Hot: 80-100" && l.Score >= 80 || Filters[1] == "Warm: 50-79" && l.Score >= 50 && l.Score < 80 || Filters[1] == "Cold: 0-49" && l.Score < 50)
                && (Filters[2] == "All sources" || l.Source == Filters[2]) && (Filters[3] == "All companies" || l.Company == Filters[3]) && (Filters[4] == "All owners" || l.Owner == Filters[4])
                && (Filters[5] == "All values" || Filters[5] == "Under $10K" && l.Value < 10000 || Filters[5] == "$10K-$50K" && l.Value >= 10000 && l.Value <= 50000 || Filters[5] == "$50K-$100K" && l.Value > 50000 && l.Value <= 100000 || Filters[5] == "$100K+" && l.Value > 100000)
                && (Filters[6] == "Any follow-up" || l.FollowKind == Filters[6]) && (Filters[7] == "All tags" || l.Tags.Contains(Filters[7])));
            return (Filters[8] switch { "Highest Score" => rows.OrderByDescending(l => l.Score), "Lowest Score" => rows.OrderBy(l => l.Score), "Highest Value" => rows.OrderByDescending(l => l.Value), "Next Follow-up" => rows.OrderBy(l => l.Follow, StringComparer.Create(CultureInfo.GetCultureInfo("en-US"), false)), "Company A-Z" => rows.OrderBy(l => l.Company, StringComparer.Create(CultureInfo.GetCultureInfo("en-US"), false)), "Lead Name A-Z" => rows.OrderBy(l => l.Name, StringComparer.Create(CultureInfo.GetCultureInfo("en-US"), false)), _ => rows.OrderByDescending(l => l.Id) }).ToArray();
        }
    }
    public CrmLead[] Visible => Filtered.Take(PageSize).ToArray();
    public bool AllSelected => Visible.Length > 0 && Visible.All(l => Selected.Contains(l.Id));
    public void Reset() { Query = ""; for (var i = 0; i < Filters.Length; i++) Filters[i] = Data.Filters[i][0]; }
    public void Toggle(long id) { if (!Selected.Remove(id)) Selected.Add(id); }
    public void SelectAll() { var clear = AllSelected; Selected.Clear(); if (!clear) Selected.AddRange(Visible.Select(l => l.Id)); }
    public void BeginAdd() => Draft = Data.Blank with { };
    public void BeginEdit(CrmLead lead) { Active = lead; var name = lead.Name.Split(' '); Draft = new() { First = name[0], Last = string.Join(' ', name.Skip(1)), Email = lead.Email, Phone = lead.Phone, Role = lead.Role, Company = lead.Company, Stage = lead.Stage, Source = lead.Source, Owner = lead.Owner, Score = lead.Score.ToString(CultureInfo.InvariantCulture), Value = lead.Value.ToString(CultureInfo.InvariantCulture), Probability = lead.Probability.ToString(CultureInfo.InvariantCulture), Follow = lead.Follow, Tags = string.Join(", ", lead.Tags), Notes = string.Join(' ', lead.Notes) }; }
    public void Update(long id, Func<CrmLead, CrmLead> patch) { for (var i = 0; i < Leads.Count; i++) if (Leads[i].Id == id) Leads[i] = patch(Leads[i]); if (Active?.Id == id) Active = patch(Active); }
    public void Bulk(Func<CrmLead, CrmLead> patch) { for (var i = 0; i < Leads.Count; i++) if (Selected.Contains(Leads[i].Id)) Leads[i] = patch(Leads[i]); }
    public bool Save(bool add)
    {
        var score = JavaScriptNumber.Parse(Draft.Score); var value = JavaScriptNumber.Parse(Draft.Value); var probability = JavaScriptNumber.Parse(Draft.Probability);
        if (Draft.First.Length == 0 || Draft.Last.Length == 0 || Draft.Email.Length > 0 && !Regex.IsMatch(Draft.Email, @"^\S+@\S+\.\S+$") || score < 0 || score > 100 || value < 0 || probability < 0 || probability > 100) return false;
        CrmLead Patch(CrmLead l) => l with { Name = $"{Draft.First} {Draft.Last}", Email = Draft.Email, Phone = Draft.Phone, Role = Draft.Role, Company = Draft.Company, Stage = Draft.Stage, Source = Draft.Source, Owner = Draft.Owner, Score = score, Value = value, Probability = probability, Follow = Draft.Follow, Tags = Draft.Tags.Split(',').Select(t => t.Trim()).Where(t => t.Length > 0).ToArray(), Notes = Draft.Notes.Length > 0 ? [Draft.Notes] : [] };
        if (add) { var initials = $"{Draft.First[0]}{Draft.Last[0]}".ToUpperInvariant(); Leads.Add(Patch(new() { Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), FollowKind = "This Week", Activity = "Lead created", Initials = initials })); }
        else if (Active is not null) Update(Active.Id, Patch); return true;
    }
    public bool Confirm(string mode)
    {
        if (mode == "bulkOwner") Bulk(l => l with { Owner = BulkValue });
        else if (mode == "bulkStage") Bulk(l => l with { Stage = BulkValue });
        else if (mode == "bulkTag") Bulk(l => l with { Tags = [.. l.Tags, BulkValue] });
        else if (mode == "bulkDelete") { Leads.RemoveAll(l => Selected.Contains(l.Id)); Selected.Clear(); }
        else if (Active is not null) { var id = Active.Id; if (mode == "note" && Text.Length > 0) Update(id, l => l with { Notes = [.. l.Notes, Text] }); else if (mode == "task" && Text.Length > 0) Update(id, l => l with { Tasks = [.. l.Tasks, Text] }); else if (mode == "owner") Update(id, l => l with { Owner = BulkValue }); else if (mode == "stage") Update(id, l => l with { Stage = BulkValue }); else if (mode == "convert") Update(id, l => l with { Stage = "Converted" }); else if (mode == "disqualify") Update(id, l => l with { Stage = "Disqualified" }); else if (mode == "archive") Update(id, l => l with { Stage = "Archived" }); else if (mode == "delete") { Leads.RemoveAll(l => l.Id == id); Active = null; } else return false; } else return false;
        Text = ""; return true;
    }
    public string Export(bool selected = false) { var rows = selected ? Leads.Where(l => Selected.Contains(l.Id)) : Filtered; return "Lead,Company,Score,Stage,Value,Owner" + string.Concat(rows.Select(l => "\n" + string.Join(',', l.Name, l.Company, l.Score.ToString(CultureInfo.InvariantCulture), l.Stage, l.Value.ToString(CultureInfo.InvariantCulture), l.Owner))); }
}
