using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record LmsCourse(string Id, string Title, string Category, string InstructorId, int Lessons, int Students, int Completed, double Price, string Status, string Color);
public sealed record LmsInstructor(string Id, string Name, string Specialty, string Rating, string ImageName);
public sealed record LmsLearner(string Id, string Name, string CourseId, int Finished, string LastActive);
public sealed record LmsSession(string Id, string Title, string InstructorId, string Category, string Date, string Time, string Duration, string Day, string Month, string Description);
public sealed record LmsOverview(LmsCourse[] Selected, int Enrollments, int Completions, double Revenue, double CompletionRate, int[] EnrollmentSeries, int[] CompletionSeries, string[] Labels);
public sealed record LmsSourceData(string[] Categories, LmsCourse[] Courses, LmsInstructor[] Instructors, LmsLearner[] Learners, LmsSession[] Sessions)
{
    public static LmsSourceData Load() { using var stream = typeof(LmsSourceData).Assembly.GetManifestResourceStream("Rynzo.LmsSource") ?? throw new InvalidOperationException("LMS source unavailable."); return JsonSerializer.Deserialize<LmsSourceData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("LMS source invalid."); }
}
public sealed class LmsDraft
{
    public string Title { get; set; } = "";
    public string Category { get; set; } = "Development";
    public string InstructorId { get; set; } = "daniel";
    public string Lessons { get; set; } = "12";
    public string Price { get; set; } = "49";
}
