using System.Text.Json;
namespace Rynzo.Client.Features.Ecommerce.Models;

public sealed record ProductDetailsData(string[] Gallery, string[] Finishes, string[][] Features, string[][] Specifications, string[][] Reviews, int[][] Distribution, string[][] Shipping, string[][] Recommendations)
{
    public static ProductDetailsData Load() { using var stream = typeof(ProductDetailsData).Assembly.GetManifestResourceStream("Rynzo.ProductDetailsSource")!; return JsonSerializer.Deserialize<ProductDetailsData>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web))!; }
}
public sealed class ProductPurchaseState
{
    public int Quantity { get; private set; } = 1;
    public bool InCart { get; set; }
    public bool Wishlisted { get; set; }
    public string Finish { get; set; } = "Graphite";
    public void Increase() => Quantity = Math.Min(24, Quantity + 1);
    public void Decrease() => Quantity = Math.Max(1, Quantity - 1);
    public string CartLabel => InCart ? $"{Quantity} item{(Quantity > 1 ? "s" : "")} in cart" : "Add to cart";
}
