using System.Text.Json;
using System.Net.Mail;
namespace Rynzo.Client.Features.School.Models;

public record Student(string Id, string Name, string ClassId, string Guardian, string Email, string Status, string Photo);
public record SchoolClass(string Id, string Title, string Teacher, string Room, int Capacity, int Subjects, string Color);
public record Exam(string Id, string Subject, string ClassId, string Date, string Time, int Duration, string Room, string Status);
public record SchoolData(List<Student> Students, SchoolClass[] Classes, List<Exam> Exams);
public record AttendanceEntry(string Status = "Unmarked", string Note = "", int Minutes = 0);
public sealed class SchoolState
{
    public SchoolData Data { get; } = JsonSerializer.Deserialize<SchoolData>(SchoolSource.Json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    public static readonly string[] Statuses = ["Present", "Late", "Absent", "Excused", "Unmarked"];
    private readonly Dictionary<string, Dictionary<string, AttendanceEntry>> Registers = new() { ["2026-10-09"] = new[] { "Present", "Late", "Present", "Excused", "Present", "Absent", "Present", "Present", "Present", "Present" }.Select((s, i) => (Id: $"STU-{101 + i}", Entry: new AttendanceEntry(s))).ToDictionary(x => x.Id, x => x.Entry) };
    private (string Date, Dictionary<string, AttendanceEntry> Entries)? UndoEntry;
    public AttendanceEntry Entry(string date, string id) => Registers.GetValueOrDefault(date)?.GetValueOrDefault(id) ?? new();
    public bool CanUndo(string date) => UndoEntry?.Date == date;
    public void Change(string date, Dictionary<string, AttendanceEntry> changes) { if (!DateOnly.TryParse(date, out _)) return; var old = Registers.GetValueOrDefault(date) ?? []; UndoEntry = (date, new(old)); var next = new Dictionary<string, AttendanceEntry>(old); foreach (var c in changes) next[c.Key] = c.Value; Registers[date] = next; }
    public void Apply(string date, IEnumerable<string> ids, string status) => Change(date, ids.ToDictionary(id => id, id => Entry(date, id) with { Status = status, Minutes = status == "Late" ? Entry(date, id).Minutes : 0 }));
    public void Undo(string date) { if (UndoEntry is { } undo && undo.Date == date) { Registers[date] = undo.Entries; UndoEntry = null; } }
    public string? Review(string date, string id, AttendanceEntry draft) { if (draft.Status == "Late" && (draft.Minutes < 0 || draft.Minutes > 240)) return "Late minutes must be a whole number from 0 to 240."; Change(date, new() { [id] = draft with { Note = draft.Note.Trim(), Minutes = draft.Status == "Late" ? draft.Minutes : 0 } }); return null; }
    public int? Rate(string date, IEnumerable<Student> scope) { var entries = scope.Select(s => Entry(date, s.Id)).ToArray(); var denominator = entries.Count(e => e.Status is "Present" or "Late" or "Absent"); return denominator == 0 ? null : (int)Math.Round(entries.Count(e => e.Status is "Present" or "Late") * 100d / denominator, MidpointRounding.AwayFromZero); }
    public string? AddStudent(string name, string classId, string guardian, string email) { if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(guardian)) return "Enter the student and guardian names."; if (!MailAddress.TryCreate(email, out _)) return "Enter a valid guardian email."; var group = Data.Classes.First(c => c.Id == classId); if (Data.Students.Count(s => s.ClassId == classId) >= group.Capacity) return "This class is at capacity."; Data.Students.Add(new($"STU-{Data.Students.Max(s => int.Parse(s.Id[4..])) + 1}", name.Trim(), classId, guardian.Trim(), email, "Active", "assets/images/users/user-1.jpg")); return null; }
    public void ToggleStudent(string id) { var i = Data.Students.FindIndex(s => s.Id == id); Data.Students[i] = Data.Students[i] with { Status = Data.Students[i].Status == "Active" ? "On leave" : "Active" }; }
    public string? Schedule(Exam draft) { if (string.IsNullOrWhiteSpace(draft.Subject) || string.IsNullOrWhiteSpace(draft.Room) || draft.Duration < 30 || draft.Duration > 180) return "Enter a subject, venue, and a duration from 30 to 180 minutes."; if (!DateOnly.TryParse(draft.Date, out _) || !TimeOnly.TryParse(draft.Time, out var t)) return "Enter a valid exam date and time."; var start = t.Hour * 60 + t.Minute; if (start + draft.Duration > 1440) return "The exam must finish on the selected date."; if (Data.Exams.Any(x => x.Status == "Scheduled" && x.Date == draft.Date && (x.ClassId == draft.ClassId || x.Room.Equals(draft.Room.Trim(), StringComparison.OrdinalIgnoreCase)) && start < Minutes(x.Time) + x.Duration && start + draft.Duration > Minutes(x.Time))) return "This time overlaps a scheduled exam for the same class or venue."; Data.Exams.Add(draft with { Id = $"EXM-{Data.Exams.Max(e => int.Parse(e.Id[4..])) + 1}", Subject = draft.Subject.Trim(), Room = draft.Room.Trim(), Status = "Scheduled" }); return null; }
    private static int Minutes(string value) { var t = TimeOnly.Parse(value); return t.Hour * 60 + t.Minute; }
    public void Cancel(string id) { var i = Data.Exams.FindIndex(e => e.Id == id); Data.Exams[i] = Data.Exams[i] with { Status = "Cancelled" }; }
    public static string Csv(IEnumerable<IEnumerable<string>> rows) => string.Join("\r\n", rows.Select(r => string.Join(",", r.Select(v => "\"" + v.Replace("\"", "\"\"") + "\""))));
}
