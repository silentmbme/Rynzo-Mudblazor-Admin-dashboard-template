using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record LogisticsShipment(string Id, string Customer, string Origin, string Destination, string Status, int Weight, string Driver, string Vehicle, string Eta, int Progress)
{
    public static LogisticsShipment[] Load()
    {
        using var stream = typeof(LogisticsShipment).Assembly.GetManifestResourceStream("Rynzo.LogisticsSource") ?? throw new InvalidOperationException("Logistics source unavailable.");
        return JsonSerializer.Deserialize<LogisticsShipment[]>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Logistics source invalid.");
    }
}
