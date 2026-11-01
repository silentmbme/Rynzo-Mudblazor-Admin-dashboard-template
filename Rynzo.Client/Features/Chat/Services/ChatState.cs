using Rynzo.Client.Features.Chat.Models;
namespace Rynzo.Client.Features.Chat.Services;

public sealed class ChatState
{
    public ChatSource Source { get; } = ChatSource.Load();
    public List<Conversation> Conversations { get; }
    public string ActiveId { get; set; } = "sophia";
    public Conversation? Active => Conversations.FirstOrDefault(c => c.Id == ActiveId) ?? Conversations.FirstOrDefault();
    public string Filter { get; set; } = "All";
    public string Search { get; set; } = "";
    public string Draft { get; set; } = "";
    public ConversationMessage? Reply { get; set; }
    public ConversationMessage? Editing { get; set; }
    public ConversationAttachment? Attachment { get; set; }
    public string GroupName { get; set; } = "";
    public List<string> SelectedContacts { get; } = [];
    public ChatState() => Conversations = [.. Source.Conversations];
    public Conversation[] Displayed => Conversations.Where(c => $"{c.Name} {c.Subtitle}".Contains(Search, StringComparison.OrdinalIgnoreCase) && (Filter == "Unread" ? c.Unread > 0 : Filter == "Groups" ? c.Group : Filter == "Archived" ? c.Archived : !c.Archived)).OrderByDescending(c => c.Pinned).ToArray();
    public void UpdateActive(Func<Conversation, Conversation> update) { for (var i = 0; i < Conversations.Count; i++) if (Conversations[i].Id == ActiveId) Conversations[i] = update(Conversations[i]); }
    public void Open(string id) { ActiveId = id; UpdateActive(c => c with { Unread = 0 }); }
    public bool Send() { var text = Draft.Trim(); if (text.Length == 0 && Attachment is null || Active is null) return false; if (Editing is not null) { var id = Editing.Id; UpdateActive(c => c with { Subtitle = text, Messages = c.Messages.Select(m => m.Id == id ? m with { Text = text } : m).ToArray() }); Editing = null; Draft = ""; return true; } var active = Active; var message = new ConversationMessage { Id = "m-" + Guid.NewGuid().ToString("N"), Text = text, Reply = Reply?.Text, Attachment = Attachment }; Conversations.RemoveAll(c => c.Id == ActiveId); Conversations.Insert(0, active with { Subtitle = text.Length > 0 ? text : Attachment?.Name ?? "Attachment", Time = "Now", Messages = [.. active.Messages, message] }); Draft = ""; Reply = null; Attachment = null; return true; }
    public void React(string id, string emoji) => UpdateActive(c => c with { Messages = c.Messages.Select(m => m.Id == id ? m with { Reactions = Reaction(m, emoji) } : m).ToArray() });
    private static Dictionary<string, int> Reaction(ConversationMessage m, string emoji) { var values = m.Reactions is null ? new Dictionary<string, int>() : new(m.Reactions); values[emoji] = values.GetValueOrDefault(emoji) + 1; return values; }
    public void DeleteMessage(string id) => UpdateActive(c => c with { Messages = c.Messages.Where(m => m.Id != id).ToArray() });
    public void PinMessage(string id) => UpdateActive(c => c with { Messages = c.Messages.Select(m => m.Id == id ? m with { Pinned = "Always" } : m).ToArray() });
    public void ToggleContact(string name) { if (!SelectedContacts.Remove(name)) SelectedContacts.Add(name); }
    public void Start(string name) { var existing = Conversations.FirstOrDefault(c => c.Name == name); if (existing is not null) { Open(existing.Id); return; } var id = name.ToLowerInvariant().Replace(' ', '-'); Conversations.Insert(0, new() { Id = id, Name = name, Initials = string.Concat(name.Split(' ').Select(p => p[0])), Subtitle = "Start a conversation", Email = name.ToLowerInvariant().Replace(' ', '.') + "@rynzoadmin.com" }); ActiveId = id; }
    public bool CreateGroup() { if (string.IsNullOrWhiteSpace(GroupName) || SelectedContacts.Count == 0) return false; var id = "group-" + Guid.NewGuid().ToString("N"); Conversations.Insert(0, new() { Id = id, Name = GroupName, Initials = GroupName[..Math.Min(2, GroupName.Length)].ToUpperInvariant(), Subtitle = "Group created", Group = true, Email = "group@rynzoadmin.com", Members = [.. SelectedContacts, "You"], Messages = [new() { Id = "welcome", Author = "system", Text = "Group created today", Time = "" }] }); ActiveId = id; GroupName = ""; SelectedContacts.Clear(); return true; }
    public void AddMembers() { UpdateActive(c => c with { Members = [.. c.Members ?? [], .. SelectedContacts] }); SelectedContacts.Clear(); }
    public void RenameGroup() { if (!string.IsNullOrWhiteSpace(GroupName)) UpdateActive(c => c with { Name = GroupName.Trim() }); SelectedContacts.Clear(); }
    public void DeleteConversation() => Conversations.RemoveAll(c => c.Id == ActiveId);
    public static string Portrait(string name) => name == "Sophia Carter" ? "assets/images/users/sophia-carter.jpg" : name is "Daniel Lee" or "You" ? "assets/images/users/user-4.jpg" : name == "Emma Wilson" ? "assets/images/users/user-3.jpg" : name == "Noah Williams" ? "assets/images/users/user-2.jpg" : "assets/images/users/user-1.jpg";
}
