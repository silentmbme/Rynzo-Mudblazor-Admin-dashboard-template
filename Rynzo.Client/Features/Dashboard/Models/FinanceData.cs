using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record FinanceData(string[][] Profitability, JsonElement[][] Margins, JsonElement[][] Expenses, string[][] Accounts, string[][] Receivables, string[][] Payables, string[][] Transactions, JsonElement[][] Sources, JsonElement[][] Goals, string[][] Summary, AnalyticsSeries[] Series)
{
    public static FinanceData Load()
    {
        using var stream = typeof(FinanceData).Assembly.GetManifestResourceStream("Rynzo.FinanceSource") ?? throw new InvalidOperationException("Finance data unavailable.");
        return JsonSerializer.Deserialize<FinanceData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Finance data invalid.");
    }
}
