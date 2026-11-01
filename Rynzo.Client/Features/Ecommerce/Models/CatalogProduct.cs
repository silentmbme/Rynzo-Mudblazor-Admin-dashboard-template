using System.Text.Json;
namespace Rynzo.Client.Features.Ecommerce.Models;

public sealed record CatalogProduct(string Name, string Sku, string Category, string Price, string Stock, string Sold, string Rating, string Status, string Brand, string Image)
{
    public bool LowStock => int.Parse(Stock, System.Globalization.CultureInfo.InvariantCulture) <= 5;
    public string SearchText => string.Join(' ', Name, Sku, Category, Price, Stock, Sold, Rating, Status, Brand, Image);
    public static CatalogProduct[] Load() { using var stream = typeof(CatalogProduct).Assembly.GetManifestResourceStream("Rynzo.ProductsSource")!; var rows = JsonSerializer.Deserialize<string[][]>(stream)!; return rows.Select(p => new CatalogProduct(p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7], p[8], p[9])).ToArray(); }
}
