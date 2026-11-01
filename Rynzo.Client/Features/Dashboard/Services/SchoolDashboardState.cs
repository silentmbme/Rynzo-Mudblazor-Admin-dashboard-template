using System.Globalization;
using Rynzo.Client.Features.Dashboard.Models;
namespace Rynzo.Client.Features.Dashboard.Services;

public sealed class SchoolDashboardState
{
    public SchoolSourceData Source { get; } = SchoolSourceData.Load();
    public List<SchoolStudent> Students { get; }
    public List<SchoolAssignment> Assignments { get; }
    public SchoolAdmission Admission { get; } = new();
    public SchoolDashboardState() { Students = [.. Source.Students]; Assignments = [.. Source.Assignments]; }
    public SchoolStudent[] Selected(string grade) => Students.Where(s => grade == "all" || s.Grade == grade).ToArray();
    // Original search deliberately does not trim the query.
    public SchoolStudent[] Filter(string grade, string query) => Selected(grade).Where(s => $"{s.Name} {s.Id}".Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    public bool Admit() { var draft = Admission; if (draft.Name.Trim().Length < 2 || draft.Name.Length > 80 || !Source.Grades.Contains(draft.Grade)) return false; Students.Add(new($"ST-{101 + Students.Count}", draft.Name.Trim(), draft.Grade, "Present", draft.Grade == "Grade 8" ? 1200 : draft.Grade == "Grade 9" ? 1400 : 1600, 0)); return true; }
    public bool ToggleAttendance(string id) { var index = Students.FindIndex(s => s.Id == id); if (index < 0) return false; var student = Students[index]; Students[index] = student with { Attendance = student.Attendance == "Present" ? "Absent" : "Present" }; return true; }
    public bool MarkPaid(string id) { var index = Students.FindIndex(s => s.Id == id && s.Paid != s.Fee); if (index < 0) return false; Students[index] = Students[index] with { Paid = Students[index].Fee }; return true; }
    public void ToggleAssignment(string title) { var index = Assignments.FindIndex(a => a.Title == title); if (index >= 0) Assignments[index] = Assignments[index] with { Done = !Assignments[index].Done }; }
    public string Csv(IEnumerable<SchoolStudent> students) { var rows = new List<string[]> { new[] { "ID", "Student", "Grade", "Attendance", "Fees USD", "Paid USD" } }; rows.AddRange(students.Select(s => new[] { s.Id, s.Name, s.Grade, s.Attendance, s.Fee.ToString(CultureInfo.InvariantCulture), s.Paid.ToString(CultureInfo.InvariantCulture) })); return string.Join("\r\n", rows.Select(row => string.Join(',', row.Select(v => "\"" + v.Replace("\"", "\"\"") + "\"")))); }
}
