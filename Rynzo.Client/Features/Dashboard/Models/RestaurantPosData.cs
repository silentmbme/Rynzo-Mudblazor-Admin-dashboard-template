using System.Text.Json;
namespace Rynzo.Client.Features.Dashboard.Models;

public sealed record RestaurantMenuItem(string Id, string Image, string Name, string Category, int Price, string Description, string Icon, string Color);
public sealed record RestaurantOrder(string Notes, string Id, string Table, string Items, double Total, string Status);
public static class RestaurantMenu
{
    public static RestaurantMenuItem[] Load() { using var stream = typeof(RestaurantMenu).Assembly.GetManifestResourceStream("Rynzo.RestaurantSource") ?? throw new InvalidOperationException("Restaurant source unavailable."); return JsonSerializer.Deserialize<RestaurantMenuItem[]>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Restaurant source invalid."); }
}
