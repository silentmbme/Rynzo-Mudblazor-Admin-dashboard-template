using System.Text.Json;
namespace Rynzo.Client.Features.Ecommerce.Models;

public sealed record CheckoutItem(int Id, string Name, string Variant, double Price, int Quantity, string Image);
public sealed record CheckoutDelivery(string Description, double Price);
public sealed record CheckoutContact { public string First { get; set; } = ""; public string Last { get; set; } = ""; public string Email { get; set; } = ""; public string Phone { get; set; } = ""; public string Company { get; set; } = ""; }
public sealed record CheckoutAddress { public string Line1 { get; set; } = ""; public string Line2 { get; set; } = ""; public string City { get; set; } = ""; public string State { get; set; } = ""; public string Postal { get; set; } = ""; public string Country { get; set; } = ""; }
// Public source demo defaults only. Never persisted or sent to a payment service.
public sealed record CheckoutCard { public string Holder { get; set; } = ""; public string Number { get; set; } = ""; public string Expiry { get; set; } = ""; public string Cvv { get; set; } = ""; }
public sealed record CheckoutTotals(double Subtotal, double Shipping, double Discount, double Tax, double Total)
{
    public static string Money(double value) => "$" + value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
}
public sealed record CheckoutSource(CheckoutItem[] BaseItems, Dictionary<string, CheckoutDelivery> DeliveryInfo, CheckoutContact InitialContact, CheckoutAddress InitialAddress, CheckoutCard DemoCard)
{
    public static CheckoutSource Load() { using var stream = typeof(CheckoutSource).Assembly.GetManifestResourceStream("Rynzo.CheckoutSource")!; return JsonSerializer.Deserialize<CheckoutSource>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web))!; }
}
