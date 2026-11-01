using System.Globalization;
using Rynzo.Client.Features.Dashboard.Models;
namespace Rynzo.Client.Features.Dashboard.Services;

public static class AiDashboardCalculator
{
    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");
    // All rounded quantities are nonnegative. Match JavaScript Math.round, not banker's rounding.
    private static long Round(double value) => (long)Math.Floor(value + .5);
    public static AiDashboardData Calculate(AiSourceData source, int period, string modelId)
    {
        if (period is not (7 or 30 or 90)) throw new ArgumentOutOfRangeException(nameof(period));
        var factor = period / 30d;
        var models = source.Models.Where(m => modelId == "all" || m.Id == modelId).Select(m => m with { Requests = Round(m.Requests * factor), Tokens = Round(m.Tokens * factor), Cost = Round(m.Cost * factor * 100) / 100d }).ToArray();
        var requests = models.Sum(m => m.Requests); var tokens = models.Sum(m => m.Tokens); var cost = models.Sum(m => m.Cost);
        var denominator = requests == 0 ? 1 : requests;
        var latency = models.Sum(m => m.Latency * m.Requests) / denominator;
        var success = models.Sum(m => m.Success * m.Requests) / denominator;
        var change = models.Sum(m => m.Change * m.Requests) / denominator;
        var buckets = period == 7 ? 7 : 12;
        var weights = source.DailyWeights.Take(buckets).ToArray(); var weightTotal = weights.Sum();
        long[] Conserve(IEnumerable<long> values, long total) { var result = values.ToArray(); result[^1] += total - result.Sum(); return result; }
        var requestSeries = Conserve(weights.Select(weight => (long)Math.Floor(weight / weightTotal * requests)), requests);
        var tokenSeries = Conserve(requestSeries.Select(value => (long)Math.Floor((double)value * tokens / denominator)), tokens);
        var failedRequests = Round(requests * (1 - success / 100));
        var failedSeries = Conserve(requestSeries.Select(value => (long)Math.Floor(value * (1 - success / 100))), failedRequests);
        var inputTokens = Round(tokens * .6); var cachedTokens = Round(tokens * .12); var outputTokens = tokens - inputTokens - cachedTokens;
        var inputSeries = Conserve(tokenSeries.Select(value => (long)Math.Floor(value * .6)), inputTokens);
        var cachedSeries = Conserve(tokenSeries.Select(value => (long)Math.Floor(value * .12)), cachedTokens);
        var outputSeries = tokenSeries.Select((value, index) => value - inputSeries[index] - cachedSeries[index]).ToArray();
        var labels = Enumerable.Range(0, buckets).Select(index => new DateTime(2026, 10, 9, 0, 0, 0, DateTimeKind.Utc).AddDays(-Round((buckets - 1 - index) * (period - 1d) / (buckets - 1))).ToString("MMM d", English)).ToArray();
        var dayNames = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" }; var hours = new[] { "00–04", "04–08", "08–12", "12–16", "16–20", "20–24" };
        var activityTotal = source.ActivityWeights.Sum() * source.DayWeights.Sum();
        var activity = dayNames.Select((name, day) => new AiHeatRow(name, source.ActivityWeights.Select((weight, index) => new AiHeatPoint(hours[index], (long)Math.Floor(requests * weight * source.DayWeights[day] / activityTotal))).ToArray())).ToArray();
        activity[^1].Data[^1] = activity[^1].Data[^1] with { Y = activity[^1].Data[^1].Y + requests - activity.Sum(day => day.Data.Sum(point => point.Y)) };
        return new() { Models = models, Requests = requests, Tokens = tokens, Cost = cost, Latency = latency, Success = success, Change = change, RequestSeries = requestSeries, TokenSeries = tokenSeries, FailedRequests = failedRequests, FailedSeries = failedSeries, InputTokens = inputTokens, OutputTokens = outputTokens, CachedTokens = cachedTokens, InputTokenSeries = inputSeries, CachedTokenSeries = cachedSeries, OutputTokenSeries = outputSeries, Activity = activity, Labels = labels };
    }
    public static string Number(double value)
    {
        var magnitude = Math.Abs(value);
        var scale = magnitude >= 999_950_000_000 ? 1_000_000_000_000d : magnitude >= 999_950_000 ? 1_000_000_000d : magnitude >= 999_950 ? 1_000_000d : magnitude >= 999.95 ? 1_000d : 1d;
        return Math.Round(value / scale, 1, MidpointRounding.AwayFromZero).ToString("0.#", English) + (scale == 1_000_000_000_000 ? "T" : scale == 1_000_000_000 ? "B" : scale == 1_000_000 ? "M" : scale == 1_000 ? "K" : "");
    }
    public static string Cost(double value) => value.ToString("C2", English);
    public static string Csv(AiDashboardData data, int period)
    {
        var rows = new List<string[]> { new[] { "Model", "Period (days)", "Requests", "Tokens", "Cost (USD)", "Latency (seconds)", "Success rate (%)" } };
        rows.AddRange(data.Models.Select(model => new[] { model.Name, period.ToString(CultureInfo.InvariantCulture), model.Requests.ToString(CultureInfo.InvariantCulture), model.Tokens.ToString(CultureInfo.InvariantCulture), model.Cost.ToString("F2", CultureInfo.InvariantCulture), model.Latency.ToString(CultureInfo.InvariantCulture), model.Success.ToString(CultureInfo.InvariantCulture) }));
        return string.Join("\r\n", rows.Select(row => string.Join(',', row.Select(value => "\"" + value.Replace("\"", "\"\"") + "\""))));
    }
}
