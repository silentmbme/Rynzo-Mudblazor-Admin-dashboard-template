using Rynzo.Components;
using Rynzo.Client.Infrastructure.Http;
using Rynzo.Client.Infrastructure;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRynzoFoundation();
if (builder.Environment.IsDevelopment())
{
    // Some local launch contexts cannot write to the Windows Event Log. Keeping
    // only console logging prevents a logging failure from aborting page requests.
    builder.Logging.ClearProviders();
    builder.Logging.AddSimpleConsole();

}
builder.Services.AddOptions<ExpressApiOptions>()
    .BindConfiguration(ExpressApiOptions.SectionName)
    .Validate(options => options.IsValid(), "ExpressApi requires a valid HTTP(S) base URL or blank pending configuration, and a timeout of 1-300 seconds.")
    .ValidateOnStart();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

if (builder.Environment.IsDevelopment())
{
    // Use an in-memory key ring so local development does not depend on user-profile
    // DPAPI keys that may be unavailable or unreadable in the current environment.
    builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
// The HTTP development profile has no HTTPS endpoint to redirect to.
var hasDevelopmentHttpsEndpoint =
    (builder.Configuration["urls"] ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries)
        .Any(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) ||
    !string.IsNullOrWhiteSpace(builder.Configuration["HTTPS_PORT"]) ||
    !string.IsNullOrWhiteSpace(builder.Configuration["HTTPS_PORTS"]);
if (!app.Environment.IsDevelopment() || hasDevelopmentHttpsEndpoint)
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Rynzo.Client._Imports).Assembly);

app.Run();
