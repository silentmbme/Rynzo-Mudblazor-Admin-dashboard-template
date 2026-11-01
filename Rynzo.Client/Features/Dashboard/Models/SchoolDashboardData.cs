using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record SchoolStudent(string Id, string Name, string Grade, string Attendance, int Fee, int Paid);
public sealed record SchoolTeacher(string Name, string Subject, string Grade);
public sealed record SchoolAssignment(string Title, string Grade, string Subject, bool Done);
public sealed record SchoolEvent(string Title, string Day, string Weekday, string Type, string Detail, string Color);
public sealed record SchoolBus(string Route, string Bus, string Color, string BarColor, int Seats, int Total, string Status);
public sealed record SchoolBook(string Title, string Due);
public sealed record SchoolSourceData(string[] Grades, SchoolStudent[] Students, SchoolTeacher[] Teachers, SchoolAssignment[] Assignments, SchoolEvent[] Events, SchoolBus[] Buses, SchoolBook[] Books)
{
    public static SchoolSourceData Load() { using var stream = typeof(SchoolSourceData).Assembly.GetManifestResourceStream("Rynzo.SchoolSource") ?? throw new InvalidOperationException("School source unavailable."); return JsonSerializer.Deserialize<SchoolSourceData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("School source invalid."); }
}
public sealed class SchoolAdmission { public string Name { get; set; } = ""; public string Grade { get; set; } = "Grade 8"; }
