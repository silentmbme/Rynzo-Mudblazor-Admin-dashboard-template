using Rynzo.Client.Features.Tasks.Models;
namespace Rynzo.Client.Features.Tasks.Services;

public sealed class TasksState
{
    public TasksSource Source { get; } = TasksSource.Load();
    public List<WorkTask> Tasks { get; }
    public List<string> Selected { get; } = [];
    public string Search { get; set; } = "";
    public string View { get; set; } = "All Tasks";
    public string Status { get; set; } = "All";
    public string Priority { get; set; } = "All";
    public string Assignee { get; set; } = "All";
    public string Sort { get; set; } = "Due date";
    public string Group { get; set; } = "None";
    public string CommentDraft { get; set; } = "";
    public string? DraggedId { get; set; }
    public TasksState() => Tasks = [.. Source.Tasks];
    public WorkTask[] Shown
    {
        get { var rows = Tasks.Where(t => MatchesView(t) && (Status == "All" || t.Status == Status) && (Priority == "All" || t.Priority == Priority) && (Assignee == "All" || t.Assignees.Contains(Assignee)) && $"{t.Title} {t.Description} {t.Project} {string.Join(' ', t.Tags)} {string.Join(' ', t.Assignees)}".Contains(Search, StringComparison.OrdinalIgnoreCase)); return Sort == "Title A-Z" ? rows.OrderBy(t => t.Title, StringComparer.Create(System.Globalization.CultureInfo.GetCultureInfo("en-US"), false)).ToArray() : Sort == "Priority" ? rows.OrderBy(t => Array.IndexOf(Source.Priorities, t.Priority)).ToArray() : rows.OrderBy(t => t.Due, StringComparer.Ordinal).ToArray(); }
    }
    private bool MatchesView(WorkTask t) => View == "All Tasks" || View == "Today" && t.Due == "2026-08-24" || View == "Upcoming" && string.CompareOrdinal(t.Due, "2026-08-24") > 0 && t.Status != "Completed" || View == "Overdue" && string.CompareOrdinal(t.Due, "2026-08-24") < 0 && t.Status != "Completed" || View == "Completed" && t.Status == "Completed";
    public int ViewCount(string view) => Tasks.Count(t => view == "All Tasks" || view == "Today" && t.Due == "2026-08-24" && t.Status != "Completed" || view == "Upcoming" && string.CompareOrdinal(t.Due, "2026-08-24") > 0 && t.Status != "Completed" || view == "Overdue" && string.CompareOrdinal(t.Due, "2026-08-24") < 0 && t.Status != "Completed" || view == "Completed" && t.Status == "Completed");
    public (string Name, WorkTask[] Tasks)[] Groups => Group == "None" ? [("All tasks", Shown)] : Shown.GroupBy(t => Group == "Status" ? t.Status : Group == "Priority" ? t.Priority : Group == "Assignee" ? t.Assignees.FirstOrDefault() ?? "Unassigned" : t.Project).Select(g => (g.Key, g.ToArray())).ToArray();
    public bool Save(WorkTask task) { if (string.IsNullOrWhiteSpace(task.Title)) return false; var saved = task with { Updated = "Now" }; if (task.Id.Length == 0) Tasks.Insert(0, saved with { Id = "task-" + Guid.NewGuid().ToString("N") }); else { var index = Tasks.FindIndex(t => t.Id == task.Id); if (index >= 0) Tasks[index] = saved; } return true; }
    public void ToggleSelection(string id) { if (!Selected.Remove(id)) Selected.Add(id); }
    public void SelectAll(bool value) { Selected.Clear(); if (value) Selected.AddRange(Shown.Select(t => t.Id)); }
    public void Complete(IEnumerable<string> ids) { var selected = ids.ToHashSet(); for (var i = 0; i < Tasks.Count; i++) if (selected.Contains(Tasks[i].Id)) Tasks[i] = Tasks[i] with { Status = Tasks[i].Status == "Completed" ? "Todo" : "Completed", Updated = "Now" }; Selected.Clear(); }
    public void ProgressSelected() { for (var i = 0; i < Tasks.Count; i++) if (Selected.Contains(Tasks[i].Id)) Tasks[i] = Tasks[i] with { Status = "In Progress" }; Selected.Clear(); }
    public bool Drop(string status) { if (DraggedId is null || Group != "Status" || !Source.Statuses.Contains(status)) return false; var i = Tasks.FindIndex(t => t.Id == DraggedId); if (i < 0) return false; Tasks[i] = Tasks[i] with { Status = status, Updated = "Now" }; DraggedId = null; return true; }
    public void Duplicate(WorkTask task) => Tasks.Insert(0, task with { Id = "task-" + Guid.NewGuid().ToString("N"), Title = task.Title + " (Copy)", Status = "Todo" });
    public void Delete(IEnumerable<string> ids) { var selected = ids.ToHashSet(); Tasks.RemoveAll(t => selected.Contains(t.Id)); Selected.Clear(); }
    public void ToggleSubtask(string id, int subtask) { var i = Tasks.FindIndex(t => t.Id == id); if (i >= 0) { var subs = Tasks[i].Subtasks.ToArray(); if (subtask < 0 || subtask >= subs.Length) return; subs[subtask] = subs[subtask] with { Done = !subs[subtask].Done }; Tasks[i] = Tasks[i] with { Subtasks = subs }; } }
    public bool AddComment(string id) { if (string.IsNullOrWhiteSpace(CommentDraft)) return false; var i = Tasks.FindIndex(t => t.Id == id); if (i < 0) return false; Tasks[i] = Tasks[i] with { Comments = [.. Tasks[i].Comments, new("c-" + Guid.NewGuid().ToString("N"), "You", CommentDraft)] }; CommentDraft = ""; return true; }
    public static string Portrait(string name) => name == "Sophia Carter" ? "assets/images/users/sophia-carter.jpg" : name == "Daniel Lee" ? "assets/images/users/user-4.jpg" : name == "Emma Wilson" ? "assets/images/users/user-3.jpg" : name == "Noah Williams" ? "assets/images/users/user-2.jpg" : "assets/images/users/user-1.jpg";
}
