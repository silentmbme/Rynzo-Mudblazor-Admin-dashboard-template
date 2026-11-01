using System.Globalization;
using Rynzo.Client.Features.Dashboard.Models;
namespace Rynzo.Client.Features.Dashboard.Services;

public sealed class LmsDashboardState
{
    public LmsSourceData Source { get; } = LmsSourceData.Load();
    public List<LmsCourse> Courses { get; }
    public LmsLearner[] Learners { get; }
    public LmsDraft Draft { get; } = new();
    public LmsDashboardState() { Courses = [.. Source.Courses]; Learners = [.. Source.Learners]; }
    private static int Round(double value) => (int)Math.Floor(value + .5);
    public LmsOverview Overview(int period, string category)
    {
        if (period is not (7 or 30)) throw new ArgumentOutOfRangeException(nameof(period));
        var selected = Courses.Where(c => category == "all" || c.Category == category).ToArray();
        var enrollments = selected.Sum(c => Round(c.Students * period / 30d)); var completions = selected.Sum(c => Round(c.Completed * period / 30d)); var revenue = selected.Sum(c => Round(c.Students * period / 30d) * c.Price);
        int[] Distribute(int total) { double[] weights = [.7, .86, 1.04, .92, 1.16, 1.08, 1.24]; var sum = weights.Sum(); var result = weights.Select(w => (int)Math.Floor(total * w / sum)).ToArray(); result[^1] += total - result.Sum(); return result; }
        var labels = Enumerable.Range(0, 7).Select(i => new DateTime(2026, 10, 9, 0, 0, 0, DateTimeKind.Utc).AddDays(-Round((6 - i) * (period - 1d) / 6)).ToString("MMM d", CultureInfo.GetCultureInfo("en-US"))).ToArray();
        return new(selected, enrollments, completions, revenue, enrollments == 0 ? 0 : completions / (double)enrollments * 100, Distribute(enrollments), Distribute(completions), labels);
    }
    public const string ValidationError = "Enter a course title, 1–200 lessons, and a price between $0 and $10,000.";
    public bool Create()
    {
        var draft = Draft;
        if (draft.Title.Trim().Length < 3 || draft.Title.Length > 100 || !int.TryParse(draft.Lessons, NumberStyles.Integer, CultureInfo.InvariantCulture, out var lessons) || lessons < 1 || lessons > 200 || !double.TryParse(draft.Price, NumberStyles.Float, CultureInfo.InvariantCulture, out var price) || !double.IsFinite(price) || price < 0 || price > 10000 || !Source.Categories.Contains(draft.Category) || !Source.Instructors.Any(i => i.Id == draft.InstructorId)) return false;
        Courses.Insert(0, new($"CRS-{101 + Courses.Count}", draft.Title.Trim(), draft.Category, draft.InstructorId, lessons, 0, 0, price, "Draft", "--primary")); return true;
    }
    public string? CompleteLesson(string id)
    {
        var index = Array.FindIndex(Learners, l => l.Id == id); if (index < 0) return null; var learner = Learners[index]; var courseIndex = Courses.FindIndex(c => c.Id == learner.CourseId); if (courseIndex < 0) return null; var course = Courses[courseIndex]; if (learner.Finished >= course.Lessons) return null;
        Learners[index] = learner with { Finished = learner.Finished + 1 }; var completed = learner.Finished + 1 == course.Lessons; if (completed) Courses[courseIndex] = course with { Completed = course.Completed + 1 }; return completed ? "Sample course completed" : "Sample lesson marked complete";
    }
    public LmsCourse[] Filter(LmsOverview data, string status, string query) => data.Selected.Where(c => (status == "all" || c.Status == status) && $"{c.Title} {c.Category}".Contains(query.Trim(), StringComparison.OrdinalIgnoreCase)).ToArray();
    public string Csv(IEnumerable<LmsCourse> courses) { var rows = new List<string[]> { new[] { "Course", "Category", "Instructor", "Lessons", "Enrollments (30-day sample)", "Completed", "Price (USD)", "Status" } }; rows.AddRange(courses.Select(c => new[] { c.Title, c.Category, Source.Instructors.Single(i => i.Id == c.InstructorId).Name, c.Lessons.ToString(CultureInfo.InvariantCulture), c.Students.ToString(CultureInfo.InvariantCulture), c.Completed.ToString(CultureInfo.InvariantCulture), c.Price.ToString(CultureInfo.InvariantCulture), c.Status })); return string.Join("\r\n", rows.Select(row => string.Join(',', row.Select(v => "\"" + v.Replace("\"", "\"\"") + "\"")))); }
}
