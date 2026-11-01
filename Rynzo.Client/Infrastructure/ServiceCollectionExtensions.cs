using Rynzo.Client.Layout;
using Rynzo.Client.Theme;
using Rynzo.Client.Infrastructure.Http;
using MudBlazor.Services;
namespace Rynzo.Client.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRynzoFoundation(this IServiceCollection services)
    {
        services.AddMudServices(options => options.ResizeOptions.BreakpointDefinitions = RynzoTheme.BreakpointDefinitions);
        services.AddScoped<RynzoAppearance>();
        services.AddScoped<LayoutSession>();
        services.AddScoped<ILayoutSession>(sp => sp.GetRequiredService<LayoutSession>());
        services.AddScoped<ExpressApiConfiguration>();
        services.AddScoped(_ => new HttpClient { Timeout = Timeout.InfiniteTimeSpan });
        services.AddScoped<ExpressApiClient>();
        return services;
    }
}
