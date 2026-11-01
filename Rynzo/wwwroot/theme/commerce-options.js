import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';
export function options(kind, data) {
    const base = baseChartOptions(document.documentElement.classList.contains('dark'));
    const marginOptions = mergeOptions(base, {
        chart: {
            type: "line",
            toolbar: {
                show: false,
            },
        },
        colors: [token("--chart-1"), token("--muted-foreground")],
        stroke: {
            curve: "smooth",
            width: [2.5, 1.5],
            dashArray: [0, 6],
        },
        fill: { opacity: [0.1, 1], type: ["solid", "solid"] },
        dataLabels: {
            enabled: false,
        },
        xaxis: {
            categories: ["Aug 1", "5", "9", "13", "17", "21", "25", "Today"],
        },
        yaxis: {
            min: 30,
            max: 70,
            labels: {
                formatter: (value) => `${value}%`,
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
        tooltip: {
            y: {
                formatter: (value) => `${value.toFixed(1)}%`,
            },
        },
    });
    const basketOptions = mergeOptions(base, {
        chart: {
            type: "donut",
        },
        labels: ["Single item", "2 items", "3 items", "4+ items"],
        colors: [
            token("--chart-1"),
            token("--chart-2"),
            token("--muted-foreground"),
            token("--chart-4"),
        ],
        legend: {
            show: false,
        },
        dataLabels: {
            enabled: false,
        },
        stroke: {
            width: 4, colors: [token("--card")],
        },
        plotOptions: {
            pie: {
                donut: {
                    size: "80%",
                    labels: {
                        show: true,
                        total: {
                            show: true,
                            label: "Items / order",
                            formatter: () => "2.6",
                        },
                    },
                },
            },
        },
    });
    return { ...flatChartOptions({ margin: marginOptions, basket: basketOptions }[kind], chartPalette(), false), series: data[kind + 'Series'] };
}
