using System.Text.Json;
namespace Rynzo.Client.Features.Tasks.Models;

public sealed record WorkSubtask(string Title, bool Done);
public sealed record WorkComment(string Id, string Author, string Text);
public sealed record WorkTask
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Project { get; set; } = "Website Redesign";
    public string Status { get; set; } = "Todo";
    public string Priority { get; set; } = "Medium";
    public string[] Assignees { get; set; } = [];
    public string Due { get; set; } = "2026-08-25";
    public string[] Tags { get; set; } = [];
    public WorkSubtask[] Subtasks { get; set; } = [];
    public WorkComment[] Comments { get; set; } = [];
    public string[] Attachments { get; set; } = [];
    public string Reminder { get; set; } = "30 minutes before";
    public string Created { get; set; } = "Now";
    public string Updated { get; set; } = "Now";
    public string PriorityTone => Priority switch { "Critical" => "destructive", "High" => "warning", "Medium" => "info", _ => "muted-foreground" };
    public string StatusTone => Status switch { "Todo" => "info", "In Progress" => "primary", "Review" => "warning", "Completed" => "success", _ => "muted-foreground" };
}
public sealed record TasksSource(string[] People, string[] Statuses, string[] Priorities, WorkTask[] Tasks)
{
    public static TasksSource Load() { using var stream = typeof(TasksSource).Assembly.GetManifestResourceStream("Rynzo.TasksSource") ?? throw new InvalidOperationException("Tasks source unavailable."); return JsonSerializer.Deserialize<TasksSource>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("Tasks source invalid."); }
}
