# Rynzo documentation

This folder contains the standalone HTML guides for the Rynzo .NET 10 and Blazor workspace. Open `index.html` to read them locally, or serve the folder from a static web host.

## Regenerate the documentation

From the repository root, run:

```powershell
node scripts/sync-documentation.cjs
```

The generator updates every HTML guide, copies the active Rynzo logo and favicon, and rebuilds `pages.html` from Razor `@page` directives under `Rynzo.Client`. It uses Node.js built-in modules only; there is no npm install step. Run `node --check scripts/sync-documentation.cjs` to check the generator syntax.

After changing the shared theme source, use `node scripts/sync-design.cjs` separately to regenerate the application theme assets. Documentation generation does not edit the application theme.
