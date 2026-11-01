using Rynzo.Client.Features.Notes.Models;
namespace Rynzo.Client.Features.Notes.Services;

public sealed class NotesState
{
    public List<NoteEntry> Notes { get; } = [.. NoteEntry.Load()];
    public static readonly string[] Categories = ["All", "Pinned", "Work", "Personal", "Ideas"];
    public static readonly string[] Colors = ["--primary", "--info", "--success", "--warning", "--chart-5"];
    public NoteEntry[] Filter(string category, string query) => Notes.Where(n => (category == "All" || (category == "Pinned" ? n.Pinned : n.Category == category)) && $"{n.Title} {n.Content}".Contains(query, StringComparison.OrdinalIgnoreCase)).OrderByDescending(n => n.Pinned).ToArray();
    public int Count(string category) => Notes.Count(n => category == "All" || (category == "Pinned" ? n.Pinned : n.Category == category));
    public const string TitleError = "Add a title to your note.";
    public bool Save(NoteEntry draft) { if (string.IsNullOrWhiteSpace(draft.Title) || draft.Title.Length > 100 || draft.Content.Length > 10000) return false; var note = draft with { Title = draft.Title.Trim(), Content = draft.Content.Trim() }; var index = Notes.FindIndex(n => n.Id == note.Id); if (index >= 0) Notes[index] = note; else Notes.Insert(0, note); return true; }
    public void TogglePin(string id) { var index = Notes.FindIndex(n => n.Id == id); if (index >= 0) Notes[index] = Notes[index] with { Pinned = !Notes[index].Pinned }; }
    public void Delete(string id) => Notes.RemoveAll(n => n.Id == id);
    public static int Words(string content) => System.Text.RegularExpressions.Regex.Matches(content, @"\S+").Count;
    public static string Export(NoteEntry draft) => draft.Title + "\n\n" + draft.Content;
}
