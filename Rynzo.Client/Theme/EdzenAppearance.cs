using Microsoft.JSInterop;
using MudBlazor;
namespace Rynzo.Client.Theme;

public sealed class RynzoAppearance(IJSRuntime js) : IAsyncDisposable
{
    private DotNetObjectReference<RynzoAppearance>? _reference;
    public string Skin { get; private set; } = "multicolor";
    public string Mode { get; private set; } = "light";
    public bool IsDark { get; private set; }
    public string Direction { get; private set; } = "ltr";
    public string Density { get; private set; } = "comfortable";
    public string Header { get; private set; } = "fixed";
    public string LayoutWidth { get; private set; } = "fluid";
    public string SidebarSize { get; private set; } = "default";
    public string SidebarColor { get; private set; } = "light";
    public bool SidebarCollapsed { get; private set; }
    public bool Collapsed => SidebarCollapsed || SidebarSize == "compact";
    public bool DarkSidebar => IsDark || SidebarColor == "dark";
    public MudTheme Theme { get; private set; } = RynzoTheme.Default;
    public event Action? Changed;
    public async Task InitializeAsync()
    {
        if (_reference is not null) return;
        _reference = DotNetObjectReference.Create(this);
        Update(await js.InvokeAsync<AppearanceSnapshot>("rynzoTheme.connect", _reference));
    }
    public async Task ToggleAsync() => Update(await js.InvokeAsync<AppearanceSnapshot>("rynzoTheme.setMode", IsDark ? "light" : "dark"));
    public async Task SetOptionAsync(string key, string value)
    {
        var options = new Dictionary<string, object> { [key] = value };
        // Size selection overrides a previously saved header-toggle preference.
        if (key == "sidebarSize") options["sidebarCollapsed"] = false;
        Update(await js.InvokeAsync<AppearanceSnapshot>("rynzoTheme.setOptions", options));
    }
    public Task ToggleSidebarAsync() => SetOptionAsync("sidebarSize", Collapsed ? "default" : "compact");
    public async Task ResetAsync() => Update(await js.InvokeAsync<AppearanceSnapshot>("rynzoTheme.reset"));
    [JSInvokable] public void AppearanceChanged(AppearanceSnapshot state) => Update(state);
    private void Update(AppearanceSnapshot state)
    {
        Skin = RynzoTheme.Skins.Contains(state.Skin) ? state.Skin : "multicolor";
        Mode = state.Mode; IsDark = state.IsDark; Direction = state.Direction; Density = state.Density;
        Header = state.Header; LayoutWidth = state.LayoutWidth; SidebarSize = state.SidebarSize;
        SidebarColor = state.SidebarColor; SidebarCollapsed = state.SidebarCollapsed;
        Theme = RynzoTheme.Create(Skin); Changed?.Invoke();
    }
    public async ValueTask DisposeAsync()
    {
        if (_reference is null) return;
        try { await js.InvokeVoidAsync("rynzoTheme.disconnect"); } catch (JSDisconnectedException) { }
        _reference.Dispose();
    }
    public sealed class AppearanceSnapshot
    {
        public string Skin { get; set; } = "multicolor";
        public string Mode { get; set; } = "light";
        public bool IsDark { get; set; }
        public string Direction { get; set; } = "ltr";
        public string Density { get; set; } = "comfortable";
        public string Header { get; set; } = "fixed";
        public string LayoutWidth { get; set; } = "fluid";
        public string SidebarSize { get; set; } = "default";
        public string SidebarColor { get; set; } = "light";
        public bool SidebarCollapsed { get; set; }
    }
}
