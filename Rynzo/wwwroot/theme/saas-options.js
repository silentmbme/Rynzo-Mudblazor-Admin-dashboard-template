import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';
export function options(kind, data) {
    const base = baseChartOptions(document.documentElement.classList.contains('dark'));
    const revenueOptions = mergeOptions(base, {
        chart: {
            type: "line",
            toolbar: {
                show: false,
            },
        },
        colors: [token("--chart-1"), token("--chart-3")],
        stroke: {
            curve: "smooth",
            width: [0, 3],
        },
        plotOptions: {
            bar: {
                columnWidth: "48%",
                borderRadius: 6,
            },
        },
        fill: { opacity: [0.9, 0.08], type: ["solid", "solid"] },
        dataLabels: {
            enabled: false,
        },
        xaxis: {
            categories: ["Mar", "Apr", "May", "Jun", "Jul", "Aug"],
        },
        yaxis: {
            labels: {
                formatter: (value) => `$${value}K`,
            },
        },
        tooltip: {
            y: {
                formatter: (value) => `$${value.toFixed(1)}K`,
            },
        },
        grid: {
            borderColor: token("--border"),
            strokeDashArray: 4,
        },
        legend: {
            position: "top",
            horizontalAlign: "left",
            fontSize: "11px",
        },
    });
    const cohortOptions = mergeOptions(base, {
        chart: {
            type: "heatmap",
            toolbar: {
                show: false,
            },
        },
        dataLabels: {
            enabled: true,
            formatter: value => `${Math.round(value)}%`,
            style: { fontSize: "10px" },
        },
        colors: [token("--chart-1")],
        plotOptions: {
            heatmap: {
                shadeIntensity: 0,
                radius: 5,
                colorScale: {
                    ranges: [
                        {
                            from: 0,
                            to: 60,
                            color: token("--muted"),
                            foreColor: token("--muted-foreground"),
                        },
                        {
                            from: 61,
                            to: 80,
                            color: token("--chart-2"),
                            foreColor: token("--card"),
                        },
                        {
                            from: 81,
                            to: 100,
                            color: token("--chart-1"),
                            foreColor: token("--card"),
                        },
                    ],
                },
            },
        },
        xaxis: {
            categories: ["Month 0", "M1", "M2", "M3", "M4", "M5"],
        },
        stroke: { width: 4, colors: [token("--card")] },
        tooltip: { y: { formatter: value => `${value}% retained` } },
        legend: {
            show: false,
        },
    });
    // ApexCharts heatmaps require explicit x/y points; keep every source value intact.
    const series = kind === 'cohort'
        ? data.cohortSeries.map(row => ({ name: row.name, data: row.data.map((value, index) => ({ x: index === 0 ? 'Month 0' : `M${index}`, y: value })) }))
        : data[kind + 'Series'];
    return { ...flatChartOptions({ revenue: revenueOptions, cohort: cohortOptions }[kind], chartPalette(), false), series };
}
