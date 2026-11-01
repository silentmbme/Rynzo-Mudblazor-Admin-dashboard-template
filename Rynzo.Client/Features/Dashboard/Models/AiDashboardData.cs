using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record AiModel(string Id, string Name, string Category, long Requests, long Tokens, double Cost, double Latency, double Success, double Change, string Color, string Status);
public sealed record AiWorkload(string Id, string ModelId, string Name, string Category, long Tokens, string Duration, string Status, string Time);
public sealed record AiHeatPoint(string X, long Y);
public sealed record AiHeatRow(string Name, AiHeatPoint[] Data);
public sealed record AiSourceData(AiModel[] Models, AiWorkload[] Workloads, double[] DailyWeights, double[] ActivityWeights, double[] DayWeights)
{
    public static AiSourceData Load()
    {
        using var stream = typeof(AiSourceData).Assembly.GetManifestResourceStream("Rynzo.AiSource") ?? throw new InvalidOperationException("AI source data unavailable.");
        return JsonSerializer.Deserialize<AiSourceData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("AI source data invalid.");
    }
}
public sealed class AiDashboardData
{
    public AiModel[] Models { get; init; } = [];
    public long Requests { get; init; }
    public long Tokens { get; init; }
    public double Cost { get; init; }
    public double Latency { get; init; }
    public double Success { get; init; }
    public double Change { get; init; }
    public long[] RequestSeries { get; init; } = [];
    public long[] TokenSeries { get; init; } = [];
    public long FailedRequests { get; init; }
    public long[] FailedSeries { get; init; } = [];
    public long InputTokens { get; init; }
    public long OutputTokens { get; init; }
    public long CachedTokens { get; init; }
    public long[] InputTokenSeries { get; init; } = [];
    public long[] CachedTokenSeries { get; init; } = [];
    public long[] OutputTokenSeries { get; init; } = [];
    public AiHeatRow[] Activity { get; init; } = [];
    public string[] Labels { get; init; } = [];
}
