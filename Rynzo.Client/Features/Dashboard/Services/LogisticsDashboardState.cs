using Rynzo.Client.Features.Dashboard.Models;
namespace Rynzo.Client.Features.Dashboard.Services;

public sealed class LogisticsDashboardState
{
    public LogisticsShipment[] Shipments { get; private set; } = LogisticsShipment.Load();
    public LogisticsShipment[] Filter(string query, string status) => Shipments.Where(s => (status == "All" || s.Status == status) && $"{s.Id} {s.Customer} {s.Origin} {s.Destination}".Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    public bool Deliver(string id)
    {
        var index = Array.FindIndex(Shipments, s => s.Id == id);
        if (index < 0 || Shipments[index].Status is "Pending" or "Delivered") return false;
        Shipments[index] = Shipments[index] with { Status = "Delivered", Progress = 100 }; return true;
    }
    public static string Csv(IEnumerable<LogisticsShipment> shipments)
    {
        var rows = new List<string[]> { new[] { "Shipment", "Customer", "Origin", "Destination", "Status", "Weight kg", "Progress %" } };
        rows.AddRange(shipments.Select(s => new[] { s.Id, s.Customer, s.Origin, s.Destination, s.Status, s.Weight.ToString(System.Globalization.CultureInfo.InvariantCulture), s.Progress.ToString(System.Globalization.CultureInfo.InvariantCulture) }));
        return string.Join("\r\n", rows.Select(row => string.Join(',', row.Select(v => "\"" + v.Replace("\"", "\"\"") + "\""))));
    }
}
