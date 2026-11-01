namespace Rynzo.Client.Layout;

public sealed record LayoutProfile(string DisplayName, string? Description = null, string? AvatarUrl = null);
// Presentation integration boundary only; does not authenticate users or grant permissions.
public interface ILayoutSession
{
    LayoutProfile? Profile { get; }
    bool CanSignOut { get; }
    Task SignOutAsync();
    event Action? Changed;
}
public sealed class LayoutSession : ILayoutSession
{
    private Func<Task>? _signOut;
    public LayoutProfile? Profile { get; private set; }
    public bool CanSignOut => _signOut is not null;
    public event Action? Changed;
    public void Update(LayoutProfile? profile, Func<Task>? signOut)
    {
        Profile = profile; _signOut = signOut; Changed?.Invoke();
    }
    public async Task SignOutAsync()
    {
        var handler = _signOut ?? throw new InvalidOperationException("No authentication sign-out integration is configured.");
        await handler();
        Update(null, null);
    }
}

