import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';
export function formatAiNumber(value) {
    return new Intl.NumberFormat("en-US", { notation: "compact", maximumFractionDigits: 1 }).format(value);
}
export function formatAiCost(value) {
    return new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
        maximumFractionDigits: 2,
    }).format(value);
}
export function options(kind, envelope) {
    const { data, chartMetric, budgetPercent } = envelope;
    const base = baseChartOptions(document.documentElement.classList.contains('dark'));
    const usageSeries = chartMetric === "requests"
        ? [
            {
                name: "Successful",
                data: data.requestSeries.map((value, index) => value - (data.failedSeries[index] ?? 0)),
            },
            { name: "Failed", data: data.failedSeries },
        ]
        : [
            { name: "Input", data: data.inputTokenSeries },
            { name: "Output", data: data.outputTokenSeries },
            { name: "Cached", data: data.cachedTokenSeries },
        ];
    const usageOptions = mergeOptions(base, {
        colors: chartMetric === "requests"
            ? [token("--chart-1"), token("--warning")]
            : [token("--chart-1"), token("--chart-2"), token("--chart-3")],
        chart: { type: "bar", stacked: true },
        stroke: { width: 0 },
        fill: { type: "solid", opacity: 1 },
        plotOptions: {
            bar: {
                columnWidth: "42%",
                borderRadius: 4,
                borderRadiusApplication: "end",
                borderRadiusWhenStacked: "last",
            },
        },
        legend: { show: true, position: "top", horizontalAlign: "left" },
        xaxis: { categories: data.labels, tickAmount: 6 },
        yaxis: { labels: { formatter: formatAiNumber } },
        tooltip: {
            y: { formatter: (value) => value.toLocaleString("en-US") },
        },
    });
    const distributionOptions = mergeOptions(base, {
        colors: data.models.map((model) => token(model.color)),
        chart: { type: "bar" },
        plotOptions: {
            bar: {
                horizontal: true,
                distributed: true,
                barHeight: "38%",
                borderRadius: 5,
            },
        },
        stroke: { width: 0 },
        fill: { type: "solid", opacity: 0.95 },
        legend: { show: false },
        xaxis: {
            categories: data.models.map((model) => model.name),
            tickAmount: 3,
            labels: { formatter: (value) => formatAiCost(Number(value)) },
        },
        yaxis: { labels: { maxWidth: 90 } },
        grid: {
            padding: { left: 0, right: 12 },
            xaxis: { lines: { show: true } },
            yaxis: { lines: { show: false } },
        },
        tooltip: { y: { formatter: formatAiCost } },
    });
    const latencyOptions = mergeOptions(base, {
        colors: [token("--chart-1"), token("--chart-2")],
        stroke: { curve: "smooth", width: [3, 2], dashArray: [0, 5] },
        xaxis: { categories: data.labels, tickAmount: 5 },
        yaxis: {
            min: 0,
            labels: { formatter: (value) => value.toFixed(1) + "s" },
        },
        legend: { position: "top", horizontalAlign: "left" },
        tooltip: {
            y: { formatter: (value) => value.toFixed(2) + " seconds" },
        },
    });
    const heatmapOptions = mergeOptions(base, {
        colors: [token("--chart-1")],
        stroke: { width: 3, colors: [token("--card")] },
        plotOptions: {
            heatmap: {
                radius: 5,
                shadeIntensity: 0.65,
                colorScale: {
                    min: 0,
                    max: Math.max(...data.activity.flatMap((day) => day.data.map((point) => point.y))),
                },
            },
        },
        dataLabels: { enabled: false },
        legend: { show: false },
        xaxis: {
            labels: { rotate: 0, style: { fontSize: "10px" } },
            tooltip: { enabled: false },
        },
        yaxis: { labels: { style: { fontSize: "11px" } } },
        grid: { show: false, padding: { left: 0, right: 0 } },
        tooltip: {
            y: {
                formatter: (value) => value.toLocaleString("en-US") + " requests",
            },
        },
    });
    const budgetOptions = mergeOptions(base, {
        colors: [token(budgetPercent >= 100 ? "--destructive" : "--primary")],
        labels: ["Budget used"],
        stroke: { lineCap: "round" },
        plotOptions: {
            radialBar: {
                startAngle: -125,
                endAngle: 125,
                hollow: { size: "70%" },
                track: { background: token("--muted"), strokeWidth: "100%" },
                dataLabels: {
                    name: { fontSize: "12px", offsetY: 24 },
                    value: {
                        fontSize: "28px",
                        fontWeight: 600,
                        offsetY: -15,
                        formatter: () => budgetPercent.toFixed(1) + "%",
                    },
                },
            },
        },
    });
    const selected = { usage: usageOptions, distribution: distributionOptions, latency: latencyOptions, heatmap: heatmapOptions, budget: budgetOptions }[kind], types = { usage: 'bar', distribution: 'bar', latency: 'line', heatmap: 'heatmap', budget: 'radialBar' };
    return { ...flatChartOptions(selected, chartPalette(), false), chart: { ...selected.chart, type: types[kind] }, series: ({ usage: usageSeries, distribution: [
                { name: "Spend", data: data.models.map((model) => model.cost) },
            ], latency: [
                {
                    name: "Average",
                    data: data.labels.map((_, index) => Number((data.latency * (1 + Math.sin(index * 1.8) * 0.12)).toFixed(2))),
                },
                {
                    name: "P95 (sample)",
                    data: data.labels.map((_, index) => Number((data.latency *
                        (1.85 + Math.cos(index * 1.2) * 0.18)).toFixed(2))),
                },
            ], heatmap: data.activity, budget: [Math.min(budgetPercent, 100)] })[kind] };
}
