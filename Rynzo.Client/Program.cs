using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Rynzo.Client.Infrastructure;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddRynzoFoundation();
await builder.Build().RunAsync();
