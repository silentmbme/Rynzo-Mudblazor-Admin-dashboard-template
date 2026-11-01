using System.Text.Json;
namespace Rynzo.Client.Features.Notes.Models;

public sealed record NoteEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Category { get; set; } = "Personal";
    public string Color { get; set; } = "--primary";
    public bool Pinned { get; set; }
    public static NoteEntry[] Load() { using var stream = typeof(NoteEntry).Assembly.GetManifestResourceStream("Rynzo.NotesSource") ?? throw new InvalidOperationException("Notes source unavailable."); return JsonSerializer.Deserialize<NoteEntry[]>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Notes source invalid."); }
}
