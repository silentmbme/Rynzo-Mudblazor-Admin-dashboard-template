namespace Rynzo.Client.Features.AiAnalytics.Models;

public sealed record ModelActivity(string Model, string Action, DateTime Time);
public sealed class ModelConfigurationState
{
    public List<AiModel> Models { get; } = AiAnalyticsSource.Models.Select(m => m with { }).ToList();
    public List<ModelActivity> Events { get; } = [];
    public int Changed => Models.Count(m => m != AiAnalyticsSource.Models.First(d => d.Id == m.Id));
    public static string Validate(AiModel m) { if (!double.IsFinite(m.Temperature) || m.Temperature < 0 || m.Temperature > 1) return "Temperature must be between 0 and 1."; if (m.Tokens < 256 || m.Tokens > 8192) return "Token limit must be a whole number from 256 to 8192."; if (m.Threshold < 50 || m.Threshold > 100) return "Confidence threshold must be a whole number from 50 to 100."; if (m.Timeout < 5 || m.Timeout > 120) return "Timeout must be a whole number from 5 to 120 seconds."; if (m.Retries < 0 || m.Retries > 3) return "Retries must be a whole number from 0 to 3."; if (string.IsNullOrWhiteSpace(m.Prompt) || m.Prompt.Length > 2000) return "Enter instructions between 1 and 2000 characters."; return ""; }
    public void Record(string model, string action) { Events.Insert(0, new(model, action, DateTime.Now)); if (Events.Count > 5) Events.RemoveAt(5); }
    public string Save(AiModel draft) { var issue = Validate(draft); if (issue.Length > 0) return issue; Models[Models.FindIndex(m => m.Id == draft.Id)] = draft with { Prompt = draft.Prompt.Trim() }; Record(draft.Name, "Configuration saved"); return ""; }
    public void Toggle(AiModel model, bool enabled) { Models[Models.FindIndex(m => m.Id == model.Id)] = model with { Enabled = enabled }; Record(model.Name, enabled ? "Profile activated" : "Profile paused"); }
}
