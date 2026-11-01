using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record AnalyticsData(string[] Days, AnalyticsMetric[] Metrics, AnalyticsSeries[] VisitorsSeries, AnalyticsSeries[] DeviceSeries, AnalyticsSeries[] ConversionSeries, JsonElement[][] Acquisition, string[][] Browsers, string[][] Pages, string[][] Locations)
{
    public static AnalyticsData Load()
    {
        using var stream = typeof(AnalyticsData).Assembly.GetManifestResourceStream("Rynzo.AnalyticsSource") ?? throw new InvalidOperationException("Analytics data unavailable.");
        return JsonSerializer.Deserialize<AnalyticsData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Analytics data invalid.");
    }
}
public sealed record AnalyticsMetric(string Icon, string Label, string Value, string Trend);
public sealed record AnalyticsSeries(string Name, double[] Data, string? Type);
