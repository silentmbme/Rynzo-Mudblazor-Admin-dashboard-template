using System.Globalization;

namespace Rynzo.Client.Features.Hrm.Models;

public record HrmPerson(string Id, string Name, string Role, string Department, string Email, string Status, string Joined);
public record PayRecord(string Id, string EmployeeId, int Base, int Allowance, int Deduction, string Status, string Period)
{
    public int Net => Base + Allowance - Deduction;
}
public record ReviewRecord(string Id, string EmployeeId, string Reviewer, int Rating, string Notes, string Status, string Due);

// Source: components/hrm/HrmPages.tsx. State belongs to the current page session.
public sealed class HrmRegister
{
    public static readonly string[] Departments = ["Design", "Engineering", "People", "Sales"];
    public IReadOnlyList<HrmPerson> People { get; } = [
        new("EMP-101", "Olivia Smith", "Product designer", "Design", "olivia@example.com", "Active", "2024-02-12"),
        new("EMP-102", "Daniel Reyes", "Frontend developer", "Engineering", "daniel@example.com", "Active", "2023-08-21"),
        new("EMP-103", "Emma Wilson", "HR specialist", "People", "emma@example.com", "On leave", "2024-05-06"),
        new("EMP-104", "Liam Walker", "Account manager", "Sales", "liam@example.com", "Active", "2022-11-14"),
        new("EMP-105", "Amara Okafor", "Design lead", "Design", "amara@example.com", "Active", "2022-03-07"),
        new("EMP-106", "Marcus Parker", "Platform engineer", "Engineering", "marcus@example.com", "Active", "2023-01-16"),
        new("EMP-107", "Sophie Turner", "People partner", "People", "sophie@example.com", "Active", "2024-07-01"),
        new("EMP-108", "Lukas Meyer", "Sales analyst", "Sales", "lukas@example.com", "On leave", "2023-09-11")];
    public List<PayRecord> Payroll { get; }
    public List<ReviewRecord> Reviews { get; } = [
        new("REV-101", "EMP-101", "Amara Okafor", 4, "Strong collaboration and clear design handoffs. Focus next quarter on documenting research decisions.", "Completed", "2026-10-12"),
        new("REV-102", "EMP-102", "Marcus Parker", 3, "Review delivery consistency, code quality, and knowledge sharing.", "Draft", "2026-10-15"),
        new("REV-103", "EMP-104", "Lukas Meyer", 4, "Review customer relationships and pipeline quality.", "Draft", "2026-10-20"),
        new("REV-104", "EMP-107", "Emma Wilson", 5, "Excellent onboarding support and thoughtful employee communication.", "Completed", "2026-10-22")];
    public HrmRegister() => Payroll = People.Select((p, i) => new PayRecord($"PAY-{101 + i}", p.Id, 3800 + i * 350, 300 + i * 20, 420 + i * 25, i < 3 ? "Processed" : "Pending", "October 2026")).ToList();
    public HrmPerson Person(string id) => People.First(p => p.Id == id);
    public bool Matches(string employeeId, string recordId, string recordStatus, string query, string department, string status)
    {
        var p = Person(employeeId);
        return $"{p.Name} {p.Role} {p.Id} {recordId}".Contains(query.Trim(), StringComparison.OrdinalIgnoreCase)
            && (department == "All" || department == p.Department) && (status == "All" || status == recordStatus);
    }
    public void Process(string id) { var i = Payroll.FindIndex(p => p.Id == id); Payroll[i] = Payroll[i] with { Status = "Processed" }; }
    public string? CreateReview(string employeeId, string reviewer, string due)
    {
        if (string.IsNullOrWhiteSpace(reviewer)) return "Enter a reviewer name.";
        if (!DateOnly.TryParseExact(due, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)) return "Enter a due date.";
        var id = $"REV-{Reviews.Max(r => int.Parse(r.Id[4..], CultureInfo.InvariantCulture)) + 1}";
        Reviews.Add(new(id, employeeId, reviewer.Trim(), 3, "", "Draft", due));
        return null;
    }
    public string? SaveReview(ReviewRecord draft, bool complete)
    {
        if (complete && string.IsNullOrWhiteSpace(draft.Notes)) return "Add feedback before completing the review.";
        var i = Reviews.FindIndex(r => r.Id == draft.Id);
        Reviews[i] = draft with { Notes = complete ? draft.Notes.Trim() : draft.Notes, Status = complete ? "Completed" : "Draft" };
        return null;
    }
    public static string Money(int n) => n.ToString("C0", CultureInfo.GetCultureInfo("en-US"));
    public static string Csv(IEnumerable<IEnumerable<string>> rows) => string.Join("\r\n", rows.Select(row => string.Join(",", row.Select(v => "\"" + v.Replace("\"", "\"\"") + "\""))));
}
