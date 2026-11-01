using System.Text.Json;
namespace Rynzo.Client.Features.Kanban.Models;

public sealed record BoardColumn { public string Id { get; set; } = ""; public string Name { get; set; } = ""; }
public sealed record BoardSubtask(string Title, bool Done);
public sealed record BoardTask
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Column { get; set; } = "backlog";
    public string Priority { get; set; } = "Medium";
    public string[] Assignees { get; set; } = [];
    public string[] Labels { get; set; } = [];
    public string Due { get; set; } = "Aug 30";
    public BoardSubtask[] Subtasks { get; set; } = [];
    public string[] Comments { get; set; } = [];
    public string[] Attachments { get; set; } = [];
    public string Tone => Priority switch { "Critical" => "destructive", "High" => "warning", "Medium" => "info", _ => "muted-foreground" };
}
public sealed record BoardSource(BoardColumn[] Columns, BoardTask[] Cards)
{
    public static BoardSource Load() { using var stream = typeof(BoardSource).Assembly.GetManifestResourceStream("Rynzo.KanbanSource") ?? throw new InvalidOperationException("Kanban source unavailable."); return JsonSerializer.Deserialize<BoardSource>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Kanban source invalid."); }
}
