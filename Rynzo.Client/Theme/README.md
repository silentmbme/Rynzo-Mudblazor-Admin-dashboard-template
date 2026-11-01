# Theme customization

The design source is `Rynzo.Client/Theme/design-system.json`. The generator uses Node.js built-in modules and has no npm dependencies. Running the app does not require Node.js.

Regenerate the compiled outputs from the workspace root after editing the source:

```powershell
node scripts/sync-design.cjs
```

Use `node scripts/sync-design.cjs --check` to confirm the checked-in generated files match the source. The generated files are included in the project, so running the app does not require Node.js.

- `settings.fontFamily` and `settings.headingFontFamily` supply Inter for body text, controls, charts, and headings. When changing web fonts, also update their stylesheet link in `Rynzo/Components/App.razor`; the generator does not download fonts. System fonts remain available as fallbacks.
- `settings.fontSizes` defines the typography scale. H1 through H6 descend in size. Page titles currently use H4 with an H1 HTML tag for document semantics.
- `settings.controlRadius` controls shared buttons, inputs, and MudBlazor's default radius (8px). Each palette's `--radius` controls shared cards, tables, and dialogs (16px). Existing feature-specific shapes remain local.
- `settings.spacing` defines the shared spacing scale. Page-specific layout adoption happens during each page's redesign.
- `settings.drawerWidth`, `drawerMiniWidth`, and `appbarHeight` configure MudBlazor layout defaults. Existing layout markup and CSS still specify matching dimensions; changing shell dimensions requires updating those references until the layout phase centralizes them.
- `palettes.<skin>.light` and `.dark` contain semantic CSS colors and chart colors. `--primary` references the appropriate accent shade for each mode; change that reference or the corresponding accent color. Emerald, orange, and cyan use a deeper shade in light mode to maintain contrast with white button text. Set `--secondary` and `--secondary-foreground` together. Keep foreground/background contrast readable.
- `--header`, `--sidebar`, and `--sidebar-foreground` supply the MudBlazor shell palette. The generator also resolves dark-sidebar variables independently of the current page mode. Navigation accents follow the active skin, including a dark sidebar on a light page.
- `--chart-1` through `--chart-8` supply chart series colors. The existing chart wrappers refresh when appearance changes.
- `--shadow-card`, `--shadow-sm`, and `--shadow-pop` supply shared elevation styles.

Generated outputs:

| File | Purpose |
| --- | --- |
| `Rynzo.Client/Theme/EdzenDesignTokens.g.cs` | MudBlazor color palettes used by the current client |
| `Rynzo.Client/Theme/EdzenThemeSettings.g.cs` | Font sizes, font stack, skin list, and layout defaults used by the current client |
| `Rynzo/wwwroot/theme/design-tokens.css` | Browser color palettes, radius, and shadows |
| `Rynzo/wwwroot/theme/design-system.css` | Browser font and spacing settings |

`EdzenTheme.cs` maps generated settings onto MudBlazor. `EdzenAppearance.cs` connects the appearance controls to `theme-bootstrap.js`. The existing `rynzo-admin-settings` and `rynzo-sidebar-color` preference keys remain unchanged; light, dark, system, RTL, density, and sidebar settings retain their existing behavior.

`Rynzo/wwwroot/theme/typography.css` applies heading families, native HTML type roles, numeric alignment, and mobile heading sizes. Feature styles use `--rynzo-font-*` variables so typography settings apply across dashboards and application pages. Branding artwork and monospace text retain their own presentation.

Keep the eight existing skin names. Adding a new skin also requires updating the bootstrap validation and customizer options; the generator alone does not add a selectable skin.
