using System.Text.Json;
namespace Rynzo.Client.Features.Ecommerce.Models;

public sealed record StoreOrder(string Id, string Customer, string Email, string Initials, int Items, decimal Total, string Date, string Payment, string Fulfillment, string Status, string Channel, string Tracking)
{
    public string Amount => "$" + Total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
}
public sealed record OrdersSource(StoreOrder[] Orders, Dictionary<string, string> OrderTone, Dictionary<string, string> PaymentTone, Dictionary<string, string> FulfillmentTone)
{
    public static OrdersSource Load() { using var stream = typeof(OrdersSource).Assembly.GetManifestResourceStream("Rynzo.OrdersSource")!; return JsonSerializer.Deserialize<OrdersSource>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web))!; }
}
