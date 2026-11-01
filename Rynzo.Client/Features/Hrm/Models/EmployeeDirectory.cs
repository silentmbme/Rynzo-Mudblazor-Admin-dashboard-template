using System.Net.Mail;
namespace Rynzo.Client.Features.Hrm.Models;

public record DirectoryEmployee(HrmPerson Person, string Location, string Mode, string Phone, string ManagerId, string Photo);
public sealed class EmployeeDirectory
{
    public static readonly string[] Modes = ["Office", "Hybrid", "Remote"];
    public List<DirectoryEmployee> Employees { get; } = new HrmRegister().People.Select((p, i) => new DirectoryEmployee(p, i % 3 == 0 ? "New York" : i % 3 == 1 ? "London" : "Austin", Modes[i % 3], $"+1 202-555-01{i:00}", i < 4 ? $"EMP-{105 + i}" : "", $"assets/images/users/user-{i % 4 + 1}.jpg")).ToList();
    public string Manager(DirectoryEmployee e) => Employees.FirstOrDefault(p => p.Person.Id == e.ManagerId)?.Person.Name ?? "Unassigned";
    public IEnumerable<DirectoryEmployee> Filter(string query, string department, string status, string mode, string sort)
    {
        var rows = Employees.Where(e => $"{e.Person.Name} {e.Person.Role} {e.Person.Email} {e.Person.Id} {e.Location}".Contains(query.Trim(), StringComparison.OrdinalIgnoreCase) && (department == "All" || e.Person.Department == department) && (status == "All" || e.Person.Status == status) && (mode == "All" || e.Mode == mode));
        return sort == "newest" ? rows.OrderByDescending(e => e.Person.Joined).ThenBy(e => e.Person.Name) : sort == "department" ? rows.OrderBy(e => e.Person.Department).ThenBy(e => e.Person.Name) : rows.OrderBy(e => e.Person.Name);
    }
    public string? Save(DirectoryEmployee draft, bool isNew)
    {
        if (string.IsNullOrWhiteSpace(draft.Person.Name) || string.IsNullOrWhiteSpace(draft.Person.Role) || string.IsNullOrWhiteSpace(draft.Location)) return "Enter the employee name, role, and location.";
        if (!MailAddress.TryCreate(draft.Person.Email.Trim(), out _)) return "Enter a valid email.";
        if (!DateOnly.TryParse(draft.Person.Joined, out _)) return "Enter a join date.";
        if (Employees.Any(e => e.Person.Id != draft.Person.Id && e.Person.Email.Equals(draft.Person.Email.Trim(), StringComparison.OrdinalIgnoreCase))) return "An employee with this email already exists.";
        if (draft.ManagerId.Length > 0 && draft.ManagerId == draft.Person.Id) return "An employee cannot be their own manager.";
        var id = isNew ? $"EMP-{Employees.Max(e => int.Parse(e.Person.Id[4..])) + 1}" : draft.Person.Id;
        var record = draft with { Person = draft.Person with { Id = id, Name = draft.Person.Name.Trim(), Role = draft.Person.Role.Trim(), Email = draft.Person.Email.Trim() }, Location = draft.Location.Trim(), Phone = draft.Phone.Trim() };
        if (isNew) Employees.Add(record); else Employees[Employees.FindIndex(e => e.Person.Id == id)] = record;
        return null;
    }
    public void Toggle(string id) { var i = Employees.FindIndex(e => e.Person.Id == id); var e = Employees[i]; Employees[i] = e with { Person = e.Person with { Status = e.Person.Status == "Active" ? "On leave" : "Active" } }; }
}
