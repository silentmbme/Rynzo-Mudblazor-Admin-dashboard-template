using System.Globalization;
using Rynzo.Client.Features.Dashboard.Models;
namespace Rynzo.Client.Features.Dashboard.Services;

public sealed class RestaurantPosState
{
    public RestaurantMenuItem[] Menu { get; } = RestaurantMenu.Load();
    public Dictionary<string, int> Cart { get; } = [];
    public List<RestaurantOrder> Orders { get; } = [];
    public string Table { get; set; } = "Table 01";
    public string Notes { get; set; } = "";
    public string OrderType { get; set; } = "Dine in";
    public int Discount { get; set; }
    public int Count(string id) => Cart.GetValueOrDefault(id);
    public RestaurantMenuItem[] Items => Menu.Where(i => Count(i.Id) > 0).ToArray();
    public double Subtotal => Items.Sum(i => i.Price * Count(i.Id));
    private static double Round(double value) => Math.Floor(value + .5);
    public double DiscountAmount => Round(Subtotal * Discount) / 100;
    public double Tax => Round((Subtotal - DiscountAmount) * .08 * 100) / 100;
    public double Total => Round((Subtotal - DiscountAmount + Tax) * 100) / 100;
    public void Quantity(string id, int delta) { if (Menu.Any(i => i.Id == id)) Cart[id] = Math.Clamp(Count(id) + delta, 0, 99); }
    public void Remove(string id) => Cart[id] = 0;
    public RestaurantMenuItem[] Filter(string category, string query) => Menu.Where(i => (category == "All" || i.Category == category) && $"{i.Name} {i.Description}".Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    public RestaurantOrder? PlaceOrder() { var items = Items; if (items.Length == 0) return null; var order = new RestaurantOrder(Notes.Trim(), $"ORD-{1001 + Orders.Count}", OrderType == "Dine in" ? Table : OrderType, string.Join(", ", items.Select(i => $"{Count(i.Id)} × {i.Name}")), Total, "Preparing"); Orders.Insert(0, order); Cart.Clear(); Notes = ""; return order; }
    public bool MarkReady(string id) { var index = Orders.FindIndex(o => o.Id == id && o.Status != "Ready"); if (index < 0) return false; Orders[index] = Orders[index] with { Status = "Ready" }; return true; }
    public static string Money(double value) => value.ToString("C2", CultureInfo.GetCultureInfo("en-US"));
    public static string Receipt(RestaurantOrder order) => $"Rynzo Restaurant — SAMPLE RECEIPT\n{order.Id}\n{order.Table}\n{order.Items}\nTotal (includes sample 8% tax): {Money(order.Total)}\nNo payment processed.";
}
