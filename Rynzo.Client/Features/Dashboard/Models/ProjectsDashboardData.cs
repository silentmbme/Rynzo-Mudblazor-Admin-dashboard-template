using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record ProjectsDashboardData(string[][] Projects, JsonElement[][] Workload, string[][] Deadlines, JsonElement[][] Tasks, JsonElement[][] Portfolio, JsonElement[][] Pulse, JsonElement[][] Execution, string[][] Risks, string[][] Milestones, string[][] Standup, string[][] Decisions, JsonElement[][] Capacity, string[][] Health, string[][] Rhythm, AnalyticsSeries[] ChartSeries, AnalyticsSeries[] BudgetChartSeries, AnalyticsSeries[] TeamPerformanceChartSeries)
{
    public static ProjectsDashboardData Load() { using var stream = typeof(ProjectsDashboardData).Assembly.GetManifestResourceStream("Rynzo.ProjectsSource") ?? throw new InvalidOperationException("Projects source unavailable."); return JsonSerializer.Deserialize<ProjectsDashboardData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Projects source invalid."); }
}
