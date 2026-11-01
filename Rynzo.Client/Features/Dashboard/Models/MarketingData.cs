using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record MarketingData(string[][] Campaigns, string[][] Scoreboard, string[][] Audiences, JsonElement[][] Content, string[][] Moments, AnalyticsSeries[] DemandSeries, double[] ChannelSeries, AnalyticsSeries[] ConversionSeries)
{
    public static MarketingData Load()
    {
        using var stream = typeof(MarketingData).Assembly.GetManifestResourceStream("Rynzo.MarketingSource") ?? throw new InvalidOperationException("Marketing data unavailable.");
        return JsonSerializer.Deserialize<MarketingData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Marketing data invalid.");
    }
}
