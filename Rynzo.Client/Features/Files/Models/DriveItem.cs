using System.Text.Json;
namespace Rynzo.Client.Features.Files.Models;

public sealed record DriveItem(string Id, string Name, string Type, string Size, string Modified, string Owner, string Path, bool Folder = false, bool Star = false, bool Shared = false, bool Trash = false)
{
    public static DriveItem[] Load() { using var stream = typeof(DriveItem).Assembly.GetManifestResourceStream("Rynzo.FilesSource") ?? throw new InvalidOperationException("Files source unavailable."); return JsonSerializer.Deserialize<DriveItem[]>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Files source invalid."); }
    public string Icon => Folder ? "Folder" : Type switch { "PDF" => "FileText", "Spreadsheet" => "Sheet", "Presentation" => "Presentation", "Design" => "Palette", _ => "File" };
    public string Tone => Folder ? "warning" : Type switch { "PDF" => "destructive", "Spreadsheet" => "success", "Presentation" => "chart-3", _ => "primary" };
}
