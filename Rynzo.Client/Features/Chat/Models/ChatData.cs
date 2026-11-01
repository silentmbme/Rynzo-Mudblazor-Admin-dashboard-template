using System.Text.Json;
namespace Rynzo.Client.Features.Chat.Models;

public sealed record ConversationAttachment(string Name, string Type, string? Url = null);
public sealed record ConversationMessage
{
    public string Id { get; set; } = "";
    public string Author { get; set; } = "me";
    public string Text { get; set; } = "";
    public string Time { get; set; } = "Now";
    public string? Reply { get; set; }
    public Dictionary<string, int>? Reactions { get; set; }
    public ConversationAttachment? Attachment { get; set; }
    public string? Pinned { get; set; }
}
public sealed record Conversation
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Initials { get; set; } = "";
    public string Status { get; set; } = "online";
    public string Subtitle { get; set; } = "";
    public string Time { get; set; } = "Now";
    public int Unread { get; set; }
    public bool Group { get; set; }
    public bool Muted { get; set; }
    public bool Pinned { get; set; }
    public bool Archived { get; set; }
    public string Email { get; set; } = "";
    public ConversationMessage[] Messages { get; set; } = [];
    public string[]? Members { get; set; }
}
public sealed record ChatSource(string[] Contacts, Conversation[] Conversations, string[] Reactions)
{
    public static ChatSource Load() { using var stream = typeof(ChatSource).Assembly.GetManifestResourceStream("Rynzo.ChatSource")!; return JsonSerializer.Deserialize<ChatSource>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web))!; }
}
