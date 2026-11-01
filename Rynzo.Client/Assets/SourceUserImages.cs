using System.Text.Json;
namespace Rynzo.Client.Assets;

public static class SourceUserImages
{
    private static readonly Dictionary<string, string> Images = Load();
    private static Dictionary<string, string> Load() { using var stream = typeof(SourceUserImages).Assembly.GetManifestResourceStream("Rynzo.UserImages")!; return JsonSerializer.Deserialize<Dictionary<string, string>>(stream)!; }
    public static string Get(string? person = null) => Images.GetValueOrDefault(person?.Trim().ToLowerInvariant() ?? "", "assets/images/users/user-1.jpg");
}
