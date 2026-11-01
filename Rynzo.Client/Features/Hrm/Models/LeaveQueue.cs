namespace Rynzo.Client.Features.Hrm.Models;

public record LeaveRecord(string Id, string EmployeeId, string Type, string Start, string End, string Reason, string Status)
{
    public int Days => DateOnly.Parse(End).DayNumber - DateOnly.Parse(Start).DayNumber + 1;
    public bool Covers(string day) => string.CompareOrdinal(Start, day) <= 0 && string.CompareOrdinal(End, day) >= 0;
    public bool Overlaps(LeaveRecord other) => string.CompareOrdinal(Start, other.End) <= 0 && string.CompareOrdinal(End, other.Start) >= 0;
}
public record LeaveDecision(string Note, string Status, DateTime Time);
public sealed class LeaveQueue
{
    public HrmRegister Register { get; } = new();
    public static readonly string[] Types = ["Annual leave", "Personal leave", "Sick leave"];
    public List<LeaveRecord> Requests { get; } = [
        new("LEV-101","EMP-101","Annual leave","2026-10-12","2026-10-14","Family trip","Pending"),
        new("LEV-102","EMP-103","Personal leave","2026-10-08","2026-10-10","Personal appointment","Approved"),
        new("LEV-103","EMP-106","Annual leave","2026-10-19","2026-10-23","Planned vacation","Pending"),
        new("LEV-104","EMP-108","Sick leave","2026-10-09","2026-10-09","Recovery day","Approved"),
        new("LEV-105","EMP-104","Personal leave","2026-10-16","2026-10-16","Personal commitment","Rejected"),
        new("LEV-106","EMP-107","Annual leave","2026-10-26","2026-10-28","Time off","Pending")];
    public Dictionary<string, List<LeaveDecision>> Decisions { get; } = [];
    public IEnumerable<LeaveRecord> Scope(string department) => Requests.Where(r => department == "All" || Register.Person(r.EmployeeId).Department == department);
    public IEnumerable<LeaveRecord> Filter(string query, string department, string status, string type, string sort, string? day)
    {
        var rows = Scope(department).Where(r => $"{r.Id} {Register.Person(r.EmployeeId).Name} {r.Reason}".Contains(query.Trim(), StringComparison.OrdinalIgnoreCase) && (status == "All" || r.Status == status) && (type == "All" || r.Type == type) && (day is null || r.Covers(day)));
        return sort == "pending" ? rows.OrderByDescending(r => r.Status == "Pending").ThenBy(r => r.Start) : sort == "duration" ? rows.OrderByDescending(r => r.Days) : rows.OrderBy(r => r.Start);
    }
    public IEnumerable<LeaveRecord> Conflicts(LeaveRecord active) => Requests.Where(r => r.Id != active.Id && r.Status == "Approved" && Register.Person(r.EmployeeId).Department == Register.Person(active.EmployeeId).Department && r.Overlaps(active));
    public string? Decide(string id, string status, string note, DateTime time)
    {
        if (status == "Rejected" && string.IsNullOrWhiteSpace(note)) return "Add a decision note before rejecting the request.";
        var i = Requests.FindIndex(r => r.Id == id); Requests[i] = Requests[i] with { Status = status };
        if (!Decisions.TryGetValue(id, out var history)) { history = []; Decisions[id] = history; }
        history.Insert(0, new(note.Trim(), status, time)); return null;
    }
    public string? Create(LeaveRecord draft)
    {
        if (!DateOnly.TryParse(draft.Start, out var start) || !DateOnly.TryParse(draft.End, out var end)) return "Select a valid date range.";
        if (end < start) return "End date must be on or after the start date.";
        if (string.IsNullOrWhiteSpace(draft.Reason)) return "Enter a reason for this request.";
        if (Requests.Any(r => r.EmployeeId == draft.EmployeeId && r.Status != "Rejected" && r.Overlaps(draft))) return "These dates overlap an existing pending or approved request for this employee.";
        Requests.Add(draft with { Id = $"LEV-{Requests.Max(r => int.Parse(r.Id[4..])) + 1}", Reason = draft.Reason.Trim(), Status = "Pending" }); return null;
    }
}
