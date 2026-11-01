using System.Globalization;
using Rynzo.Client.Features.Dashboard.Models;
namespace Rynzo.Client.Features.Dashboard.Services;

public static class CryptoDashboardCalculator
{
    public static CryptoHistory History(CryptoAsset asset, string range)
    {
        var count = range switch { "24h" => 24, "7d" => 14, "30d" => 30, _ => throw new ArgumentOutOfRangeException(nameof(range)) };
        var span = range == "24h" ? 24 * 3600000L : range == "7d" ? 7 * 86400000L : 30 * 86400000L;
        var end = new DateTimeOffset(2026, 10, 9, 12, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
        var growth = asset.Change / 100 * (range == "24h" ? 1 : range == "7d" ? 2.4 : 4.8); var opening = asset.Price / (1 + growth);
        var prices = Enumerable.Range(0, count).Select(index => { var progress = index / (double)(count - 1); var wave = Math.Sin(index * 1.3) * Math.Sin(progress * Math.PI) * asset.Price * .007; return new CryptoPoint((long)Math.Floor(end - span * (1 - progress) + .5), Math.Round(opening + (asset.Price - opening) * progress + wave, 2, MidpointRounding.AwayFromZero)); }).ToArray();
        var weights = Enumerable.Range(0, count).Select(index => .7 + index % 5 * .16).ToArray(); var weightTotal = weights.Sum(); var total = asset.Volume * (range == "24h" ? 1 : range == "7d" ? 7 : 30);
        var volume = prices.Select((point, index) => new CryptoPoint(point.X, Math.Floor(total * weights[index] / weightTotal))).ToArray(); volume[^1] = volume[^1] with { Y = volume[^1].Y + total - volume.Sum(point => point.Y) };
        return new(prices, volume, growth * 100);
    }
    public static string Csv(CryptoSourceData source)
    {
        var rows = new List<string[]> { new[] { "Asset", "Quantity", "Price (USD)", "Value (USD)", "Unrealized P/L (USD)" } };
        rows.AddRange(source.Assets.Select(asset => new[] { asset.Symbol, asset.Holding.ToString(CultureInfo.InvariantCulture), asset.Price.ToString(CultureInfo.InvariantCulture), (asset.Price * asset.Holding).ToString("F2", CultureInfo.InvariantCulture), ((asset.Price - asset.Entry) * asset.Holding).ToString("F2", CultureInfo.InvariantCulture) }));
        return string.Join("\r\n", rows.Select(row => string.Join(',', row.Select(value => "\"" + value.Replace("\"", "\"\"") + "\""))));
    }
}
