using Rynzo.Client.Features.Files.Models;
namespace Rynzo.Client.Features.Files.Services;

public sealed class FilesState
{
    public List<DriveItem> Items { get; } = [.. DriveItem.Load()];
    public List<string> Selected { get; } = [];
    public string Section { get; set; } = "My Drive";
    public string Search { get; set; } = "";
    public string FolderName { get; set; } = "";
    public DriveItem[] Shown => Items.Where(i => (Section == "Trash" ? i.Trash : Section == "Starred" ? i.Star : Section == "Shared with Me" ? i.Shared : !i.Trash) && $"{i.Name} {i.Type} {i.Owner}".Contains(Search, StringComparison.OrdinalIgnoreCase)).ToArray();
    public string? AddFolder() { var name = FolderName.Trim(); if (name.Length == 0) return "Folder name is required"; if (Items.Any(i => i.Folder && i.Name.Equals(name, StringComparison.OrdinalIgnoreCase))) return "A folder with this name already exists"; Items.Add(new("f" + Guid.NewGuid().ToString("N"), name, "Folder", "0 files", "Now", "You", "/", true)); FolderName = ""; return null; }
    public void ToggleSelection(string id) { if (!Selected.Remove(id)) Selected.Add(id); }
    public void StarSelected() { for (var i = 0; i < Items.Count; i++) if (Selected.Contains(Items[i].Id)) Items[i] = Items[i] with { Star = true }; }
    public void ToggleStar(string id) { var i = Items.FindIndex(item => item.Id == id); if (i >= 0) Items[i] = Items[i] with { Star = !Items[i].Star }; }
    public void Trash(IEnumerable<string> ids) { var selected = ids.ToHashSet(); for (var i = 0; i < Items.Count; i++) if (selected.Contains(Items[i].Id)) Items[i] = Items[i] with { Trash = true }; Selected.Clear(); }
    public void Rename(string id, string name) { var i = Items.FindIndex(item => item.Id == id); if (i >= 0) Items[i] = Items[i] with { Name = name }; }
    public void Upload(string name, string contentType, long size) { Items.Add(new("u" + Guid.NewGuid().ToString("N") + name, name, contentType.StartsWith("image", StringComparison.Ordinal) ? "Image" : "File", $"{Math.Max(1, Math.Floor(size / 1024d + .5))} KB", "Now", "You", "/")); }
}
