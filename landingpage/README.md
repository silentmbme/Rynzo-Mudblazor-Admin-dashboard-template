# Rynzo landing page

This is the standalone product landing page for the Rynzo .NET 10 and Blazor workspace. Open `index.html` directly or serve the repository with a static file server. The landing page links to the sibling `documentation/` guides and opens dashboard previews in the running app.

## Refresh the demo screenshots

Start Rynzo at `http://localhost:5245`, then run from the repository root:

```powershell
node scripts/capture-rynzo-demos.cjs
```

The capture script uses headless Chrome at a 1440 × 1000 viewport and updates the 16 dashboard and 8 app screenshots plus their route manifests under `assets/screenshots/`. Set `CHROME_PATH` if Chrome is installed in a different location.
