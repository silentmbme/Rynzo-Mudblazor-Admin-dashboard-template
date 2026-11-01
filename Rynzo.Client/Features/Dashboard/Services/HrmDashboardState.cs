using System.Globalization;
using Rynzo.Client.Features.Dashboard.Models;
namespace Rynzo.Client.Features.Dashboard.Services;

public sealed class HrmDashboardState
{
    public HrmSourceData Source { get; } = HrmSourceData.Load();
    public List<HrmEmployee> Employees { get; }
    public HrmLeave[] Requests { get; }
    public HrmOnboarding[] Onboarding { get; }
    public HrmDashboardState() { Employees = [.. Source.Employees]; Requests = [.. Source.Leave]; Onboarding = [.. Source.Onboarding]; }
    public const string ValidationError = "Enter a name, role, and monthly salary between $1 and $100,000.";
    public bool Add(string name, string role, string department, string salary)
    {
        if (name.Trim().Length < 2 || role.Trim().Length < 2 || name.Length > 80 || role.Length > 80 || !double.TryParse(salary, NumberStyles.Float, CultureInfo.InvariantCulture, out var amount) || !double.IsFinite(amount) || amount < 1 || amount > 100000 || !Source.Departments.Contains(department)) return false;
        Employees.Add(new($"EMP-{101 + Employees.Count}", name.Trim(), role.Trim(), department, "Present", amount, "user-1")); return true;
    }
    public bool Review(int id, string status) { var index = Array.FindIndex(Requests, r => r.Id == id && r.Status == "Pending"); if (index < 0 || status is not ("Approved" or "Declined")) return false; Requests[index] = Requests[index] with { Status = status }; return true; }
    public void ToggleOnboarding(string title) { var index = Array.FindIndex(Onboarding, t => t.Title == title); if (index >= 0) Onboarding[index] = Onboarding[index] with { Done = !Onboarding[index].Done }; }
    public HrmEmployee[] Selected(string department) => Employees.Where(e => department == "all" || e.Department == department).ToArray();
    public HrmEmployee[] Visible(string department, string query) => Selected(department).Where(e => $"{e.Name} {e.Role}".Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    public static string Csv(IEnumerable<HrmEmployee> employees) { var rows = new List<string[]> { new[] { "Employee ID", "Name", "Role", "Department", "Attendance", "Monthly salary (USD)" } }; rows.AddRange(employees.Select(e => new[] { e.Id, e.Name, e.Role, e.Department, e.Status, e.Salary.ToString(CultureInfo.InvariantCulture) })); return string.Join("\r\n", rows.Select(row => string.Join(',', row.Select(v => "\"" + v.Replace("\"", "\"\"") + "\"")))); }
}
