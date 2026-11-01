function clamp01(v) {
    return v < 0 ? 0 : v > 1 ? 1 : v;
}
function srgbChannel(c) {
    const v = c <= 0.0031308 ? 12.92 * c : 1.055 * Math.pow(c, 1 / 2.4) - 0.055;
    return Math.round(clamp01(v) * 255);
}
function oklchToHex(input) {
    const match = /oklch\(\s*([\d.%]+)\s+([\d.]+)\s+([\d.]+)/i.exec(input);
    if (!match)
        return null;
    const rawL = match[1] ?? "0";
    const L = rawL.endsWith("%") ? parseFloat(rawL) / 100 : parseFloat(rawL);
    const C = parseFloat(match[2] ?? "0");
    const H = (parseFloat(match[3] ?? "0") * Math.PI) / 180;
    const a = C * Math.cos(H);
    const b = C * Math.sin(H);
    const l_ = L + 0.3963377774 * a + 0.2158037573 * b;
    const m_ = L - 0.1055613458 * a - 0.0638541728 * b;
    const s_ = L - 0.0894841775 * a - 1.291485548 * b;
    const l = l_ * l_ * l_;
    const m = m_ * m_ * m_;
    const s = s_ * s_ * s_;
    const r = 4.0767416621 * l - 3.3077115913 * m + 0.2309699292 * s;
    const g = -1.2684380046 * l + 2.6097574011 * m - 0.3413193965 * s;
    const bl = -0.0041960863 * l - 0.7034186147 * m + 1.707614701 * s;
    return `#${[r, g, bl].map((c) => srgbChannel(c).toString(16).padStart(2, "0")).join("")}`;
}
export function token(name, fallback = "#0e7974") {
    if (typeof window === "undefined")
        return fallback;
    const raw = getComputedStyle(document.documentElement)
        .getPropertyValue(name)
        .trim();
    if (!raw)
        return fallback;
    if (raw.startsWith("#") || raw.startsWith("rgb"))
        return raw;
    if (raw.startsWith("oklch"))
        return oklchToHex(raw) ?? fallback;
    if (raw.startsWith("var(")) {
        const inner = /var\(\s*(--[\w-]+)/.exec(raw)?.[1];
        return inner ? token(inner, fallback) : fallback;
    }
    return raw;
}
export function chartPalette() {
    return [
        token("--chart-1", "#009e7a"),
        token("--chart-2", "#f26509"),
        token("--chart-3", "#7932b1"),
        token("--chart-4", "#0d9dd3"),
        token("--chart-5", "#d3860c"),
        token("--chart-6", "#519792"),
        token("--chart-7", "#e6954b"),
        token("--chart-8", "#4569ce"),
    ];
}
export function baseChartOptions(dark) {
    const grid = token("--border", dark ? "#344055" : "#e5e7eb");
    const label = token("--muted-foreground", dark ? "#a9b4c6" : "#697586");
    return {
        chart: {
            fontFamily: getComputedStyle(document.documentElement).getPropertyValue("--rynzo-font-family").trim() || "system-ui, sans-serif",
            foreColor: label,
            toolbar: { show: false },
            zoom: { enabled: false },
            animations: { enabled: true, speed: 400 },
            background: "transparent",
            parentHeightOffset: 0,
        },
        theme: { mode: dark ? "dark" : "light" },
        colors: chartPalette(),
        grid: {
            borderColor: grid,
            strokeDashArray: 4,
            padding: { left: 8, right: 8, top: 0, bottom: 0 },
            xaxis: { lines: { show: false } },
        },
        dataLabels: { enabled: false },
        stroke: { curve: "smooth", width: 2.5, lineCap: "round" },
        legend: {
            position: "top",
            horizontalAlign: "right",
            fontSize: "12px",
            labels: { colors: label },
            markers: { size: 5, strokeWidth: 0 },
            itemMargin: { horizontal: 8 },
        },
        tooltip: { theme: dark ? "dark" : "light", style: { fontSize: "12px" } },
        xaxis: {
            axisBorder: { show: false },
            axisTicks: { show: false },
            labels: { style: { fontSize: "12px", colors: label } },
            crosshairs: { stroke: { dashArray: 4, color: grid } },
        },
        yaxis: { labels: { style: { fontSize: "12px", colors: label } } },
        states: { hover: { filter: { type: "lighten" } } },
    };
}
export function sparklineOptions(color) {
    return {
        chart: {
            sparkline: { enabled: true },
            toolbar: { show: false },
            animations: { enabled: false },
            background: "transparent",
        },
        colors: [color],
        stroke: { curve: "smooth", width: 2 },
        fill: {
            type: "solid",
            opacity: 0.12,
        },
        tooltip: { enabled: false },
    };
}
export function mergeOptions(base, override) {
    const out = { ...base };
    for (const [key, value] of Object.entries(override)) {
        const existing = out[key];
        if (value &&
            typeof value === "object" &&
            !Array.isArray(value) &&
            existing &&
            typeof existing === "object" &&
            !Array.isArray(existing)) {
            out[key] = mergeOptions(existing, value);
        }
        else {
            out[key] = value;
        }
    }
    return out;
}
/** Keep page-specific series, thresholds, and axes while sharing flat chart styling. */
export function flatChartOptions(options, palette, area) {
    const { gradient: _gradient, ...fill } = options.fill ?? {};
    const softSparkline = area && options.chart?.sparkline?.enabled;
    return {
        ...options,
        colors: options.colors?.length ? options.colors : palette,
        chart: {
            ...options.chart,
            dropShadow: { ...options.chart?.dropShadow, enabled: false },
        },
        fill: {
            ...fill,
            type: softSparkline
                ? "gradient"
                : Array.isArray(fill.type)
                    ? fill.type.map(() => "solid")
                    : "solid",
            opacity: fill.opacity ?? (area ? 0.18 : 1),
            ...(softSparkline
                ? {
                    gradient: {
                        shadeIntensity: 0,
                        opacityFrom: 0.18,
                        opacityTo: 0,
                        stops: [0, 100],
                    },
                }
                : {}),
        },
    };
}
