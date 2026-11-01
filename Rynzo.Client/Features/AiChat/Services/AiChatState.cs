using Rynzo.Client.Features.AiChat.Models;
namespace Rynzo.Client.Features.AiChat.Services;

public sealed class AiChatState
{
    public AiChatSource Source { get; } = AiChatSource.Load();
    public List<ChatConversation> Conversations { get; } = [new("chat-1", "New conversation", [])];
    public string ActiveId { get; private set; } = "chat-1";
    public string Draft { get; set; } = "";
    public List<ChatAttachment> Attachments { get; } = [];
    public ChatConversation Active => Conversations.First(c => c.Id == ActiveId);
    public void NewChat() { var id = Guid.NewGuid().ToString(); Conversations.Insert(0, new(id, "New conversation", [])); Switch(id); }
    public void Switch(string id) { if (Conversations.All(c => c.Id != id)) return; ActiveId = id; Draft = ""; Attachments.Clear(); }
    public string Reply(string prompt) { var query = prompt.ToLowerInvariant(); return Source.Replies[query.Contains("email") ? "email" : query.Contains("react") || query.Contains("component") ? "react" : query.Contains("plan") || query.Contains("day") ? "plan" : "fallback"]; }
    public bool Send(string text) { var content = text.Trim(); if (content.Length == 0 && Attachments.Count == 0) return false; var id = Guid.NewGuid().ToString(); var active = Active; var title = active.Messages.Length > 0 ? active.Title : content.Length > 0 ? content : Attachments[0].Name; if (title.Length > 40) title = title[..40]; var response = (Attachments.Count > 0 ? "Attachments have been added to this local conversation. Demo mode does not upload, open, or analyze their contents.\n\n" : "") + Reply(content); var messages = active.Messages.Concat(new[] { new ChatMessage(id, "user", content, Attachments.ToArray()), new ChatMessage(id + "-reply", "assistant", response) }).ToArray(); Conversations[Conversations.FindIndex(c => c.Id == ActiveId)] = active with { Title = title, Messages = messages }; Draft = ""; Attachments.Clear(); return true; }
    public bool AddAttachment(ChatAttachment attachment) { if (Attachments.Count >= 4) return false; Attachments.Add(attachment); return true; }
    public string Export() => string.Join("\n\n", Active.Messages.Select(m => (m.Role == "user" ? "You" : "Demo assistant") + ":\n" + m.Content));
}
