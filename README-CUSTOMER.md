# Rynzo Customer Delivery

This source package contains the Rynzo solution, its landing page, and customer documentation. Generated `bin` and `obj` folders, local browser profiles, screenshots used for development checks, refactor backups, and internal redesign notes are excluded.

## Run locally

Requirements: the .NET 10 SDK.

```powershell
dotnet restore Rynzo.slnx
dotnet run --project Rynzo/Rynzo.csproj --launch-profile http
```

Open `http://localhost:5245`. The ASP.NET Core host starts the Blazor WebAssembly client.

## Publish for a server

```powershell
dotnet publish Rynzo/Rynzo.csproj -c Release -o publish
```

Deploy the complete `publish` folder. A framework-dependent publish requires the .NET 10 ASP.NET Core Runtime on the server. To publish for a specific runtime without requiring that runtime to be preinstalled, use the appropriate RID and `--self-contained true`.

Set `ExpressApi__BaseUrl` to the customer's HTTPS API endpoint when configuring the production integration. Do not put customer credentials or secrets in source-controlled settings files.

## Landing page and documentation

- Landing page: `landingpage/index.html`
- Documentation: `documentation/index.html`

Keep the `landingpage` and `documentation` folders side by side so their relative guide links work. For a hosted site, serve the landing page and docs on the same origin as the Rynzo app so its workspace links resolve correctly. The page also loads Remix Icons, fonts, and some brand marks from external services, which require internet access.

The `scripts` folder contains the documentation/theme maintenance and demo screenshot capture scripts. They are not needed to run the published application.

Customer-specific licensing, support, and delivery terms should accompany this package separately.
