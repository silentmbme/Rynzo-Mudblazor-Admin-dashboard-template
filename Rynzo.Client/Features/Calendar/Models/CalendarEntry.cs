using System.Text.Json;
namespace Rynzo.Client.Features.Calendar.Models;

public sealed record CalendarEntry
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Date { get; set; } = "";
    public string Start { get; set; } = "09:00";
    public string End { get; set; } = "10:00";
    public string Category { get; set; } = "Work";
    public string Location { get; set; } = "";
    public string Description { get; set; } = "";
    public string[] Attendees { get; set; } = [];
    public string Reminder { get; set; } = "10 minutes before";
    public string Recurrence { get; set; } = "Does not repeat";
    public string? Link { get; set; }
    public string? Notes { get; set; }
    public bool AllDay { get; set; }
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    public static CalendarEntry[] Load() { using var stream = typeof(CalendarEntry).Assembly.GetManifestResourceStream("Rynzo.CalendarSource") ?? throw new InvalidOperationException("Calendar source unavailable."); return JsonSerializer.Deserialize<CalendarEntry[]>(stream, Json) ?? throw new InvalidOperationException("Calendar source invalid."); }
}
public sealed record CalendarLane(CalendarEntry Event, int Lane, int Lanes);
