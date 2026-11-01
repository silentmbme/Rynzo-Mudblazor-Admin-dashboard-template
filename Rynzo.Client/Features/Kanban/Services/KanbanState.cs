using Rynzo.Client.Features.Kanban.Models;
namespace Rynzo.Client.Features.Kanban.Services;

public sealed class KanbanState
{
    public List<BoardColumn> Columns { get; }
    public List<BoardTask> Cards { get; }
    public string Search { get; set; } = "";
    public string Priority { get; set; } = "All";
    public string? Drag { get; set; }
    public KanbanState() { var source = BoardSource.Load(); Columns = [.. source.Columns]; Cards = [.. source.Cards]; }
    public BoardTask[] Filtered => Cards.Where(c => (Priority == "All" || c.Priority == Priority) && $"{c.Title} {c.Description} {string.Join(' ', c.Labels)} {string.Join(' ', c.Assignees)}".Contains(Search, StringComparison.OrdinalIgnoreCase)).ToArray();
    public bool SaveTask(BoardTask task) { if (string.IsNullOrWhiteSpace(task.Title)) return false; if (task.Id.Length == 0) Cards.Add(task with { Id = "card-" + Guid.NewGuid().ToString("N") }); else { var index = Cards.FindIndex(c => c.Id == task.Id); if (index >= 0) Cards[index] = task; } return true; }
    public bool SaveColumn(BoardColumn column) { if (string.IsNullOrWhiteSpace(column.Name)) return false; if (column.Id.Length == 0) Columns.Add(column with { Id = "col-" + Guid.NewGuid().ToString("N") }); else { var index = Columns.FindIndex(c => c.Id == column.Id); if (index >= 0) Columns[index] = column; } return true; }
    public bool Move(string id, string column) { var index = Cards.FindIndex(c => c.Id == id); if (index < 0 || Columns.All(c => c.Id != column)) return false; Cards[index] = Cards[index] with { Column = column }; return true; }
    public bool Drop(string column) { if (Drag is null) return false; var moved = Move(Drag, column); Drag = null; return moved; }
    public void Duplicate(BoardTask task) => Cards.Add(task with { Id = "card-" + Guid.NewGuid().ToString("N"), Title = task.Title + " (Copy)" });
    public void Delete(string id) => Cards.RemoveAll(c => c.Id == id);
}
