import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';
export function options(kind, data) {
    const base = baseChartOptions(document.documentElement.classList.contains('dark'));
    const demandChart = mergeOptions(base, {
        chart: {
            type: "line",
            toolbar: { show: false },
        },
        colors: [token("--chart-1"), token("--chart-2")],
        stroke: {
            curve: "smooth",
            width: [3, 2],
            dashArray: [0, 5],
        },
        fill: { opacity: 1, type: "solid" },
        xaxis: {
            categories: [
                "Aug 1",
                "Aug 5",
                "Aug 9",
                "Aug 13",
                "Aug 17",
                "Aug 21",
                "Aug 25",
                "Aug 29",
            ],
        },
        yaxis: {
            labels: {
                formatter: (value) => `$${Math.round(value)}k`,
            },
        },
        dataLabels: {
            enabled: false,
        },
        grid: {
            borderColor: token("--border"),
            strokeDashArray: 4,
        },
        legend: {
            position: "top",
            horizontalAlign: "left",
        },
        tooltip: {
            shared: true,
            intersect: false,
            y: { formatter: (value) => `$${value}k` },
        },
    });
    const channelChart = mergeOptions(base, {
        chart: {
            type: "donut",
        },
        labels: ["Paid social", "Organic", "Email", "Partners", "Direct"],
        colors: [
            token("--chart-1"),
            token("--chart-2"),
            token("--chart-3"),
            token("--warning"),
            token("--muted-foreground"),
        ],
        stroke: {
            colors: [token("--card")],
            width: 4,
        },
        legend: {
            position: "bottom",
        },
        dataLabels: {
            enabled: false,
        },
        plotOptions: {
            pie: {
                donut: {
                    size: "76%",
                    labels: {
                        show: true,
                        total: {
                            show: true,
                            label: "Qualified leads",
                            formatter: () => "2,846",
                        },
                    },
                },
            },
        },
    });
    const conversionChart = mergeOptions(base, {
        chart: {
            type: "bar",
            toolbar: { show: false },
        },
        colors: [token("--chart-2")],
        plotOptions: {
            bar: {
                horizontal: true,
                borderRadius: 4,
                barHeight: "42%",
            },
        },
        xaxis: {
            categories: ["Visits", "Engaged", "Leads", "Meetings", "Won"],
            max: 100,
            labels: {
                formatter: (value) => `${value}%`,
            },
        },
        dataLabels: {
            enabled: true,
            formatter: (value) => `${value}%`,
        },
        tooltip: { y: { formatter: (value) => `${value}% of visits` } },
        grid: {
            borderColor: token("--border"),
            strokeDashArray: 4,
        },
    });
    return { ...flatChartOptions({ demand: demandChart, channel: channelChart, conversion: conversionChart }[kind], chartPalette(), kind === 'demand'), series: data[kind + 'Series'] };
}
