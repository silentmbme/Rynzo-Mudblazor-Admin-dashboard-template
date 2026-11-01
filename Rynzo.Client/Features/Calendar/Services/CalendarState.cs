using System.Globalization;
using System.Text;
using System.Text.Json;
using Rynzo.Client.Features.Calendar.Models;
namespace Rynzo.Client.Features.Calendar.Services;

public sealed class CalendarState
{
    public static readonly string[] Categories = ["Work", "Meetings", "Personal", "Project", "Marketing", "Deadline", "Reminder"];
    public DateOnly Today { get; private set; }
    public DateOnly Current { get; set; }
    public string View { get; set; } = "Month";
    public string Search { get; set; } = "";
    public Dictionary<string, bool> Visible { get; } = Categories.ToDictionary(c => c, _ => true);
    public List<CalendarEntry> Events { get; private set; } = [];
    public static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    public CalendarState(DateOnly today) { Today = Current = today; var anchor = WeekStart(today); Events = CalendarEntry.Load().Select(e => e with { Date = Iso(anchor.AddDays(Parse(e.Date).DayNumber - new DateOnly(2026, 8, 24).DayNumber)) }).ToList(); }
    public static string Iso(DateOnly value) => value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    public static DateOnly Parse(string date) => DateOnly.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    public static DateOnly WeekStart(DateOnly date) => date.AddDays(-((int)date.DayOfWeek + 6) % 7);
    public DateOnly[] Days => PeriodDays(Current, View);
    public static DateOnly[] PeriodDays(DateOnly current, string view) { var start = view == "Month" ? new DateOnly(current.Year, current.Month, 1) : view == "Day" ? current : WeekStart(current); var count = view == "Month" ? DateTime.DaysInMonth(current.Year, current.Month) : view == "Day" ? 1 : 7; return Enumerable.Range(0, count).Select(start.AddDays).ToArray(); }
    public CalendarEntry[] Filtered => Events.Where(e => Visible.GetValueOrDefault(e.Category) && $"{e.Title} {e.Location} {e.Description} {string.Join(' ', e.Attendees)}".Contains(Search.Trim(), StringComparison.OrdinalIgnoreCase)).OrderBy(e => e.Date, StringComparer.Ordinal).ThenByDescending(e => e.AllDay).ThenBy(e => e.Start, StringComparer.Ordinal).ToArray();
    public CalendarEntry[] Period => Filtered.Where(e => string.CompareOrdinal(e.Date, Iso(Days[0])) >= 0 && string.CompareOrdinal(e.Date, Iso(Days[^1])) <= 0).ToArray();
    public double Hours => Period.Where(e => !e.AllDay).Sum(e => Minutes(e.End) - Minutes(e.Start)) / 60d;
    public void Navigate(int amount) => Current = View == "Month" ? Current.AddMonths(amount) : Current.AddDays(amount * (View == "Day" ? 1 : 7));
    public void ShowAll() { foreach (var category in Categories) Visible[category] = true; }
    public CalendarEntry Create(DateOnly date, string start = "09:00") { var end = Math.Min(Minutes(start) + 60, 1439); return new() { Date = Iso(date), Start = start, End = $"{end / 60:00}:{end % 60:00}", Link = "", Notes = "" }; }
    public static int Minutes(string value) { var parts = value.Split(':'); return int.Parse(parts[0], CultureInfo.InvariantCulture) * 60 + int.Parse(parts[1], CultureInfo.InvariantCulture); }
    public static string NormalizeTime(string value) => value.Length == 8 && value.EndsWith(":00", StringComparison.Ordinal) ? value[..5] : value;
    public static string? Validate(CalendarEntry item) { if (string.IsNullOrWhiteSpace(item.Title)) return "Give your event a title."; if (!DateOnly.TryParseExact(item.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)) return "Choose a valid date."; if (!item.AllDay) { if (!TimeOnly.TryParseExact(item.Start, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _) || !TimeOnly.TryParseExact(item.End, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)) return "Choose a start and end time."; if (Minutes(item.End) <= Minutes(item.Start)) return "End time must be after start time."; } if (!string.IsNullOrEmpty(item.Link) && !IsMeetingLink(item.Link)) return "Meeting links must start with https:// or http://."; return null; }
    public static bool IsMeetingLink(string link) => link.StartsWith("https://", StringComparison.OrdinalIgnoreCase) || link.StartsWith("http://", StringComparison.OrdinalIgnoreCase);
    public (CalendarEntry Saved, bool Collision) Save(CalendarEntry draft) { var error = Validate(draft); if (error is not null) throw new ArgumentException(error, nameof(draft)); var saved = draft with { Title = draft.Title.Trim(), Id = draft.Id.Length == 0 ? Guid.NewGuid().ToString() : draft.Id }; var collision = Events.Any(e => e.Id != saved.Id && e.Date == saved.Date && !e.AllDay && !saved.AllDay && Minutes(e.Start) < Minutes(saved.End) && Minutes(e.End) > Minutes(saved.Start)); if (draft.Id.Length == 0) Events.Add(saved); else { var index = Events.FindIndex(e => e.Id == saved.Id); if (index >= 0) Events[index] = saved; } Current = Parse(saved.Date); return (saved, collision); }
    public void Delete(string id) => Events.RemoveAll(e => e.Id == id);
    public bool Restore(string? json)
    {
        if (json is null) return false;
        try { using var document = JsonDocument.Parse(json); if (document.RootElement.ValueKind != JsonValueKind.Array) return false; foreach (var item in document.RootElement.EnumerateArray()) { if (item.ValueKind != JsonValueKind.Object) return false; foreach (var field in new[] { "id", "title", "date", "start", "end", "description", "location" }) if (!item.TryGetProperty(field, out var property) || property.ValueKind != JsonValueKind.String) return false; if (!item.TryGetProperty("attendees", out var attendees) || attendees.ValueKind != JsonValueKind.Array || attendees.EnumerateArray().Any(a => a.ValueKind != JsonValueKind.String)) return false; if (!item.TryGetProperty("category", out var category) || category.ValueKind != JsonValueKind.String || !Categories.Contains(category.GetString())) return false; } var parsed = JsonSerializer.Deserialize<List<CalendarEntry>>(json, CalendarEntry.Json); if (parsed is null || parsed.Any(e => Validate(e) is not null)) return false; Events = parsed; return true; } catch (JsonException) { return false; }
    }
    public string Serialize() => JsonSerializer.Serialize(Events, CalendarEntry.Json);
    public static CalendarLane[] Layout(IEnumerable<CalendarEntry> events)
    {
        var sorted = events.Where(e => !e.AllDay).OrderBy(e => Minutes(e.Start)).ThenByDescending(e => Minutes(e.End)); var result = new List<CalendarLane>(); var group = new List<CalendarLane>(); var ends = new List<int>(); var groupEnd = -1;
        void Flush() { result.AddRange(group.Select(g => g with { Lanes = ends.Count })); group.Clear(); ends.Clear(); }
        foreach (var item in sorted) { var start = Minutes(item.Start); var end = Math.Max(Minutes(item.End), start + 30); if (start >= groupEnd) Flush(); var lane = ends.FindIndex(e => e <= start); if (lane < 0) { lane = ends.Count; ends.Add(end); } else ends[lane] = end; group.Add(new(item, lane, 0)); groupEnd = Math.Max(group.Count == 1 ? end : groupEnd, end); }
        Flush(); return result.ToArray();
    }
    public static string Ics(IEnumerable<CalendarEntry> events, DateTimeOffset? timestamp = null)
    {
        static string Escape(string value) => value.Replace("\\", "\\\\").Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n").Replace(";", "\\;").Replace(",", "\\,");
        var lines = new List<string> { "BEGIN:VCALENDAR", "VERSION:2.0", "PRODID:-//Rynzo//Calendar//EN", "CALSCALE:GREGORIAN", "X-WR-CALNAME:Rynzo Calendar" }; var stamp = (timestamp ?? DateTimeOffset.UtcNow).UtcDateTime.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);
        foreach (var item in events) { var date = item.Date.Replace("-", ""); lines.AddRange(["BEGIN:VEVENT", $"UID:{Escape(item.Id)}@rynzo-calendar", $"DTSTAMP:{stamp}"]); if (item.AllDay) lines.AddRange([$"DTSTART;VALUE=DATE:{date}", $"DTEND;VALUE=DATE:{Parse(item.Date).AddDays(1).ToString("yyyyMMdd", CultureInfo.InvariantCulture)}"]); else lines.AddRange([$"DTSTART:{date}T{item.Start.Replace(":", "")}00", $"DTEND:{date}T{item.End.Replace(":", "")}00"]); lines.AddRange([$"SUMMARY:{Escape(item.Title)}", $"DESCRIPTION:{Escape(item.Description)}", $"LOCATION:{Escape(item.Location)}", $"CATEGORIES:{Escape(item.Category)}", "END:VEVENT"]); }
        lines.Add("END:VCALENDAR"); return string.Join("\r\n", lines.Select(line => { var folded = new StringBuilder(); var length = 0; foreach (var rune in line.EnumerateRunes()) { var size = rune.Utf8SequenceLength; if (length + size > 75) { folded.Append("\r\n "); length = 1; } folded.Append(rune); length += size; } return folded.ToString(); })) + "\r\n";
    }
    public static string Tone(string category) => category switch { "Work" => "chart-4", "Meetings" => "primary", "Personal" => "success", "Project" => "chart-3", "Marketing" => "chart-5", "Deadline" => "destructive", _ => "muted-foreground" };
}


