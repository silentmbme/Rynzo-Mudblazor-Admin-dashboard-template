using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record CrmPriorityDeal(string Name, string Company, string Value, string Likelihood, string Due, string Next, string Color);
public sealed record CrmDashboardData(CrmPriorityDeal[] PriorityDeals, string[][] Meetings, string[][] Signals, string[][] Pipeline, string[][] Conversations, string[][] Committee, string[][] Routing, string[][] Renewals, string[][] Coaching, JsonElement[] MapSeries)
{
    public static CrmDashboardData Load() { using var stream = typeof(CrmDashboardData).Assembly.GetManifestResourceStream("Rynzo.CrmSource") ?? throw new InvalidOperationException("CRM source unavailable."); return JsonSerializer.Deserialize<CrmDashboardData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("CRM source invalid."); }
}
