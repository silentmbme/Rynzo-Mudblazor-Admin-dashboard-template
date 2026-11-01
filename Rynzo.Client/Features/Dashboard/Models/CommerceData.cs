using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record CommerceData(string[][] Orders, string[][] Baskets, string[][] Stages, string[][] Carriers, string[][] Signals, string[][] Recovery, string[][] Returns, string[][] Customers, AnalyticsSeries[] MarginSeries, double[] BasketSeries)
{
    public static CommerceData Load()
    {
        using var stream = typeof(CommerceData).Assembly.GetManifestResourceStream("Rynzo.CommerceSource") ?? throw new InvalidOperationException("Commerce data unavailable.");
        return JsonSerializer.Deserialize<CommerceData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Commerce data invalid.");
    }
}
