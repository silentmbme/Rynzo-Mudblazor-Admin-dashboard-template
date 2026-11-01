using System.Globalization;
namespace Rynzo.Client.Features.Ecommerce.Models;

public sealed class ProductDraft
{
    public string Name { get; set; } = "";
    public string Status { get; set; } = "Draft";
    public string Sku { get; set; } = "WHP-1048";
    public string Brand { get; set; } = "Rynzo";
    public string Category { get; set; } = "Electronics";
    public string Type { get; set; } = "Physical product";
    public string Description { get; set; } = "Premium wireless headphones with noise cancellation, all-day comfort, and 40-hour battery life.";
    public string Price { get; set; } = "129.00";
    public string CompareAt { get; set; } = "159.00";
    public string Cost { get; set; } = "64.00";
    public bool TrackInventory { get; set; } = true;
    public string Stock { get; set; } = "24";
    public string Reorder { get; set; } = "10";
    public string Weight { get; set; } = "0.42";
    public string Collections { get; set; } = "";
    public string Tags { get; set; } = "audio, wireless, premium";
    public string Channel { get; set; } = "Online store";
    public List<string> Media { get; } = ["assets/images/products/wireless-headphones-pro.png"];
    public static readonly string[] Gallery = ["assets/images/products/smart-watch-series-x.png", "assets/images/products/leather-backpack.png", "assets/images/products/running-shoes-x2.png", "assets/images/products/portable-speaker-mini.png"];
    private static double Number(string value) => double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var n) && double.IsFinite(n) ? n : 0;
    public string Margin => (Number(Price) > 0 ? Math.Max(0, (Number(Price) - Number(Cost)) / Number(Price) * 100) : 0).ToString("F1", CultureInfo.InvariantCulture);
    public string Selling => Number(Price).ToString("F2", CultureInfo.InvariantCulture);
    public string ItemCost => Number(Cost).ToString("F2", CultureInfo.InvariantCulture);
    public string Profit => Math.Max(0, Number(Price) - Number(Cost)).ToString("F2", CultureInfo.InvariantCulture);
    public bool Save(string status) { if (string.IsNullOrWhiteSpace(Name)) return false; Status = status; return true; }
    public void Cover(string image) { Media.RemoveAll(i => i == image); Media.Insert(0, image); }
    public void AddMedia(int count) { if (count > 0) Media.Add(Gallery[0]); }
}
