using MudBlazor;
namespace Rynzo.Client.Theme;

public static class RynzoTheme
{
    public static readonly string[] Skins = RynzoThemeSettings.Skins;
    public static readonly string[] FontFamily = RynzoThemeSettings.FontFamily;
    public static Dictionary<Breakpoint, int> BreakpointDefinitions => new()
    {
        [Breakpoint.Xs] = 0,
        [Breakpoint.Sm] = 640,
        [Breakpoint.Md] = 768,
        [Breakpoint.Lg] = 1024,
        [Breakpoint.Xl] = 1280,
        [Breakpoint.Xxl] = 1536
    };
    public static MudTheme Default { get; } = Create("multicolor");
    public static MudTheme Create(string skin)
    {
        var light = new PaletteLight();
        var dark = new PaletteDark();
        Apply(light, RynzoDesignTokens.Get(skin, false), false);
        Apply(dark, RynzoDesignTokens.Get(skin, true), true);
        return new MudTheme
        {
            PaletteLight = light,
            PaletteDark = dark,
            Typography = new Typography
            {
                Default = new DefaultTypography { FontFamily = FontFamily, FontSize = RynzoThemeSettings.DefaultFontSize, FontWeight = "400", LineHeight = "1.5", LetterSpacing = "normal" },
                H1 = new H1Typography { FontFamily = RynzoThemeSettings.HeadingFontFamily, FontSize = RynzoThemeSettings.H1FontSize, FontWeight = "700", LineHeight = "1.25", LetterSpacing = "-.025em" },
                H2 = new H2Typography { FontFamily = RynzoThemeSettings.HeadingFontFamily, FontSize = RynzoThemeSettings.H2FontSize, FontWeight = "700", LineHeight = "1.3", LetterSpacing = "-.025em" },
                H3 = new H3Typography { FontFamily = RynzoThemeSettings.HeadingFontFamily, FontSize = RynzoThemeSettings.H3FontSize, FontWeight = "700", LineHeight = "1.4", LetterSpacing = "-.025em" },
                H4 = new H4Typography { FontFamily = RynzoThemeSettings.HeadingFontFamily, FontSize = RynzoThemeSettings.H4FontSize, FontWeight = "700", LineHeight = "1.25", LetterSpacing = "-.025em" },
                H5 = new H5Typography { FontFamily = RynzoThemeSettings.HeadingFontFamily, FontSize = RynzoThemeSettings.H5FontSize, FontWeight = "700", LineHeight = "1.4", LetterSpacing = "-.025em" },
                H6 = new H6Typography { FontFamily = RynzoThemeSettings.HeadingFontFamily, FontSize = RynzoThemeSettings.H6FontSize, FontWeight = "700", LineHeight = "1.5", LetterSpacing = "-.025em" },
                Body1 = new Body1Typography { FontFamily = FontFamily, FontSize = RynzoThemeSettings.Body1FontSize, FontWeight = "400", LineHeight = "1.5", LetterSpacing = "normal" },
                Body2 = new Body2Typography { FontFamily = FontFamily, FontSize = RynzoThemeSettings.Body2FontSize, FontWeight = "400", LineHeight = "1.5", LetterSpacing = "normal" },
                Subtitle1 = new Subtitle1Typography { FontFamily = FontFamily, FontSize = RynzoThemeSettings.Subtitle1FontSize, FontWeight = "500", LineHeight = "1.5", LetterSpacing = "normal" },
                Subtitle2 = new Subtitle2Typography { FontFamily = FontFamily, FontSize = RynzoThemeSettings.Subtitle2FontSize, FontWeight = "400", LineHeight = "1.5", LetterSpacing = "normal" },
                Caption = new CaptionTypography { FontFamily = FontFamily, FontSize = RynzoThemeSettings.CaptionFontSize, FontWeight = "400", LineHeight = "1.5", LetterSpacing = "normal" },
                Button = new ButtonTypography { FontFamily = FontFamily, FontSize = RynzoThemeSettings.ButtonFontSize, FontWeight = "500", LineHeight = "1.4", LetterSpacing = "normal", TextTransform = "none" },
                Overline = new OverlineTypography { FontFamily = FontFamily, FontSize = RynzoThemeSettings.OverlineFontSize, FontWeight = "600", LineHeight = "1.5", LetterSpacing = ".06em" }
            },
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = RynzoThemeSettings.ControlRadius,
                DrawerWidthLeft = RynzoThemeSettings.DrawerWidth,
                DrawerWidthRight = RynzoThemeSettings.DrawerWidth,
                DrawerMiniWidthLeft = RynzoThemeSettings.DrawerMiniWidth,
                DrawerMiniWidthRight = RynzoThemeSettings.DrawerMiniWidth,
                AppbarHeight = RynzoThemeSettings.AppbarHeight
            },
            Shadows = new Shadow { Elevation = new Shadow().Elevation.Select((_, i) => i == 0 ? "none" : i == 1 ? "var(--shadow-card)" : i == 2 ? "var(--shadow-sm)" : "var(--shadow-pop)").ToArray() }
        };
    }
    private static void Apply(Palette palette, RynzoDesignPalette tokens, bool dark)
    {
        palette.Primary = tokens.Primary;
        palette.PrimaryContrastText = tokens.PrimaryForeground;
        palette.PrimaryDarken = tokens.Accent700;
        palette.PrimaryLighten = tokens.Accent400;
        palette.Secondary = tokens.Secondary;
        palette.SecondaryContrastText = tokens.SecondaryForeground;
        palette.Background = tokens.Background;
        palette.BackgroundGray = tokens.Muted;
        palette.Surface = tokens.Card;
        palette.TextPrimary = tokens.Foreground;
        palette.TextSecondary = tokens.MutedForeground;
        palette.ActionDefault = tokens.MutedForeground;
        palette.LinesDefault = tokens.Border;
        palette.LinesInputs = tokens.Input;
        palette.Divider = tokens.Border;
        palette.TableLines = tokens.Border;
        palette.TableStriped = tokens.Muted;
        palette.TableHover = tokens.Muted;
        palette.Success = dark ? "#57d3b2" : "#0aa884";
        palette.SuccessContrastText = dark ? "#062c24" : "#ffffff";
        palette.Error = dark ? "#ff9aae" : "#ef5b78";
        palette.ErrorContrastText = dark ? "#3b0f1b" : "#ffffff";
        palette.Warning = dark ? "#ffd071" : "#e7a136";
        palette.WarningContrastText = dark ? "#372503" : "#ffffff";
        palette.Info = dark ? "#88c9ff" : "#4195dc";
        palette.InfoContrastText = dark ? "#09263d" : "#ffffff";
        // Shell colors share the same semantic source as CSS and charts.
        palette.AppbarBackground = tokens.Header;
        palette.AppbarText = tokens.Foreground;
        palette.DrawerBackground = tokens.Sidebar;
        palette.DrawerText = tokens.SidebarForeground;
        palette.DrawerIcon = tokens.Primary;
    }
}
