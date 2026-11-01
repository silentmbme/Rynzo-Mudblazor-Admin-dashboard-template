namespace Rynzo.Client.Features.Help.Models;

public sealed record FaqArticle(string Id, string Category, string Question, string Answer, IReadOnlyList<string> Keywords);
