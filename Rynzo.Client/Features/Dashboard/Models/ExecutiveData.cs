using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record ExecutiveData(string[] Months, ExecutiveKpi[] ExecutiveKpis, ExecutiveSeries[] RevenueSeries, ExecutiveSeries[] ProfitSeries, ExecutiveRegion SalesByRegion, ExecutiveProduct[] TopProducts, ExecutiveTransaction[] RecentTransactions, ExecutiveMember[] TeamPerformance, ExecutiveActivity[] ActivityFeed)
{
    public static ExecutiveData Load()
    {
        using var stream = typeof(ExecutiveData).Assembly.GetManifestResourceStream("Rynzo.ExecutiveSource") ?? throw new InvalidOperationException("Dashboard source data unavailable.");
        return JsonSerializer.Deserialize<ExecutiveData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Dashboard source data invalid.");
    }
}
public sealed record ExecutiveKpi(string Label, string Value, double Delta, string Caption, string Icon, double[] Spark);
public sealed record ExecutiveSeries(string Name, double[] Data);
public sealed record ExecutiveRegion(string[] Labels, double[] Data);
public sealed record ExecutiveProduct(string Name, string Sku, int Units, string Revenue, double Growth);
public sealed record ExecutiveTransaction(string Id, string Customer, string Method, string Date, string Amount, string Status);
public sealed record ExecutiveMember(string Name, string Role, int Quota, int Deals, string Initials);
public sealed record ExecutiveActivity(string Title, string Detail, string Time, string Tone);
