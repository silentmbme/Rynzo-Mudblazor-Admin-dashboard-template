using System.Text.Json;
namespace Rynzo.Client.Features.AiChat.Models;

public sealed record ChatAttachment(string Id, string Name, string Url, string Kind);
public sealed record ChatMessage(string Id, string Role, string Content, ChatAttachment[]? Attachments = null);
public sealed record ChatConversation(string Id, string Title, ChatMessage[] Messages);
public sealed record ChatSuggestion(string Title, string Prompt, string Color, string Icon);
public sealed record AiChatSource(ChatSuggestion[] Suggestions, Dictionary<string, string> Replies)
{
    public static AiChatSource Load() { using var stream = typeof(AiChatSource).Assembly.GetManifestResourceStream("Rynzo.AiChatSource") ?? throw new InvalidOperationException("AI chat source unavailable."); return JsonSerializer.Deserialize<AiChatSource>(stream, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? throw new InvalidOperationException("AI chat source invalid."); }
}
