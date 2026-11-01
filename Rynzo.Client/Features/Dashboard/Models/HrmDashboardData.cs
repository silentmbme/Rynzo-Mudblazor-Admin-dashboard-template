using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record HrmEmployee(string Id, string Name, string Role, string Department, string Status, double Salary, string Image);
public sealed record HrmLeave(int Id, string Name, string Department, string Type, string Dates, int Days, string Status);
public sealed record HrmOnboarding(string Title, string Department, bool Done);
public sealed record HrmPosition(string Title, string Department, int Candidates);
public sealed record HrmActivity(string Title, string Detail, string Time);
public sealed record HrmMilestone(string Name, string Label, string Day, string Department);
public sealed record HrmSourceData(string[] Departments, HrmEmployee[] Employees, HrmLeave[] Leave, HrmOnboarding[] Onboarding, HrmPosition[] Recruitment, HrmActivity[] Activity, HrmMilestone[] Milestones)
{
    public static HrmSourceData Load() { using var stream = typeof(HrmSourceData).Assembly.GetManifestResourceStream("Rynzo.HrmSource") ?? throw new InvalidOperationException("HRM source unavailable."); return JsonSerializer.Deserialize<HrmSourceData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("HRM source invalid."); }
}
