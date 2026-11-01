using System.Text.Json;
using System.Text.Json.Serialization;
namespace Rynzo.Client.Layout.Navigation;

public sealed class NavigationItem
{
    public string Label { get; set; } = "";
    public string Href { get; set; } = "/";
    public string? Icon { get; set; }
    public NavigationBadge? Badge { get; set; }
    public List<NavigationItem>? Children { get; set; }
}
public sealed class NavigationBadge { public string Text { get; set; } = ""; public string? Tone { get; set; } }
public sealed class NavigationGroup { public string Title { get; set; } = ""; public List<NavigationItem> Items { get; set; } = []; }
public sealed class HeaderShortcut { public string Label { get; set; } = ""; public string Href { get; set; } = "/"; public string Icon { get; set; } = ""; }
public sealed class SearchTarget { public string Term { get; set; } = ""; public string Href { get; set; } = "/"; }
public sealed class LayoutNotification
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Detail { get; set; } = "";
    public string Time { get; set; } = "";
    public string Tone { get; set; } = "";
    public bool Read { get; set; }
}
public sealed class LayoutCatalogData
{
    public List<NavigationGroup> Groups { get; set; } = [];
    public List<HeaderShortcut> Apps { get; set; } = [];
    public List<SearchTarget> Search { get; set; } = [];
    public List<LayoutNotification> Notifications { get; set; } = [];
}
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(LayoutCatalogData))]
internal partial class LayoutJsonContext : JsonSerializerContext { }
public static class LayoutCatalog
{
    public static LayoutCatalogData Data { get; } = Read();
    private static LayoutCatalogData Read()
    {
        using var stream = typeof(LayoutCatalog).Assembly.GetManifestResourceStream("Rynzo.LayoutSource")
            ?? throw new InvalidOperationException("Layout source catalog is missing.");
        return JsonSerializer.Deserialize(stream, LayoutJsonContext.Default.LayoutCatalogData)
            ?? throw new InvalidOperationException("Layout source catalog is invalid.");
    }
    // Deliberately preserve React's root-exact / other-prefix highlighting rule.
    public static bool IsActive(NavigationItem item, string path) => item.Href == "/" ? path == "/" : path.StartsWith(item.Href, StringComparison.Ordinal);
    public static bool BranchActive(NavigationItem item, string path) => IsActive(item, path) || item.Children?.Any(c => BranchActive(c, path)) == true;
    public static List<NavigationItem> Trail(string path)
    {
        List<NavigationItem> best = [];
        void Visit(IEnumerable<NavigationItem> items, List<NavigationItem> parents)
        {
            foreach (var item in items)
            {
                var trail = new List<NavigationItem>(parents) { item };
                if (IsActive(item, path) && (best.Count == 0 || item.Href.Length > best[^1].Href.Length || item.Href.Length == best[^1].Href.Length && trail.Count > best.Count)) best = trail;
                if (item.Children is not null) Visit(item.Children, trail);
            }
        }
        foreach (var group in Data.Groups) Visit(group.Items, []);
        return best;
    }
}

