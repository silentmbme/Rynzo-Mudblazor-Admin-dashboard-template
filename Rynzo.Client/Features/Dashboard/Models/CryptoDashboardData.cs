using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record CryptoAsset(string Symbol, string Name, string Mark, double Price, double Change, double Holding, double Entry, double Volume, double MarketCap, string Color, string Tone);
public sealed record CryptoTransaction(string Id, string Type, string Symbol, double Quantity, double Amount, string Time, string Status);
public sealed record CryptoPoint(long X, double Y);
public sealed record CryptoHistory(CryptoPoint[] Prices, CryptoPoint[] Volume, double Change);
public sealed record CryptoSourceData(CryptoAsset[] Assets, double Cash, CryptoTransaction[] Transactions)
{
    public static CryptoSourceData Load()
    {
        using var stream = typeof(CryptoSourceData).Assembly.GetManifestResourceStream("Rynzo.CryptoSource") ?? throw new InvalidOperationException("Crypto source data unavailable.");
        return JsonSerializer.Deserialize<CryptoSourceData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Crypto source data invalid.");
    }
}
