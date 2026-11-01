using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record SaasData(SaasAccount[] Accounts, SaasStat[] Stats, string[][] Movement, string[][] Activation, string[][] Health, AnalyticsSeries[] RevenueSeries, AnalyticsSeries[] CohortSeries)
{
    public static SaasData Load()
    {
        using var stream = typeof(SaasData).Assembly.GetManifestResourceStream("Rynzo.SaasSource") ?? throw new InvalidOperationException("SaaS data unavailable.");
        return JsonSerializer.Deserialize<SaasData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("SaaS data invalid.");
    }
}
public sealed record SaasAccount(string Name, string Plan, string Mrr, string Health, int Score, string Owner);
public sealed record SaasStat(string Label, string Value, double Delta, string Note, string Icon, string Color);
