import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';
export function options(kind, envelope) {
    const { data, period } = envelope, LMS_CATEGORIES = envelope.categories, base = baseChartOptions(document.documentElement.classList.contains('dark'));
    const growthOptions = mergeOptions(base, {
        chart: { stacked: false },
        colors: [token("--success"), token("--chart-1")],
        stroke: { curve: "smooth", width: [3, 2], dashArray: [0, 4] },
        fill: { opacity: [0.12, 0.04], type: "solid" },
        markers: { size: 3, strokeWidth: 2, strokeColors: token("--card"), hover: { sizeOffset: 2 } },
        xaxis: { categories: data.labels },
        yaxis: {
            min: 0,
            labels: { formatter: (value) => Math.round(value).toString() },
        },
        legend: { horizontalAlign: "left", position: "top" },
        tooltip: {
            shared: true,
            intersect: false,
            y: {
                formatter: (value) => Math.round(value).toLocaleString("en-US") + " learners",
            },
        },
    });
    const completionOptions = mergeOptions(base, {
        colors: [token("--success"), token("--chart-1")],
        stroke: { width: 0 },
        plotOptions: { bar: { horizontal: true, distributed: true, barHeight: "32%", borderRadius: 5, dataLabels: { position: "top" } } },
        xaxis: {
            categories: ["Completed", "In progress"],
            min: 0,
            max: Math.max(1, data.enrollments),
            tickAmount: 4,
            labels: { formatter: value => Math.round(Number(value)).toLocaleString("en-US") },
        },
        yaxis: { labels: { style: { fontSize: "11px" } } },
        grid: { borderColor: token("--border"), strokeDashArray: 3, xaxis: { lines: { show: true } }, yaxis: { lines: { show: false } }, padding: { left: 0, right: 20 } },
        legend: { show: false },
        dataLabels: {
            enabled: true,
            offsetX: 7,
            textAnchor: "start",
            style: { colors: [token("--foreground")], fontSize: "11px", fontWeight: 500 },
            background: { enabled: false },
            formatter: value => (data.enrollments ? value / data.enrollments * 100 : 0).toFixed(1) + "%",
        },
        tooltip: { y: { formatter: value => value.toLocaleString("en-US") + " learners" } },
    });
    const categoryOptions = mergeOptions(base, {
        colors: [token("--info")],
        stroke: { width: 2 },
        fill: { opacity: 0.08 },
        markers: { size: 3 },
        plotOptions: {
            radar: {
                polygons: {
                    strokeColors: token("--border"),
                    connectorColors: token("--border"),
                    fill: { colors: [token("--card")] },
                },
            },
        },
        xaxis: { categories: [...LMS_CATEGORIES] },
        yaxis: { min: 0, labels: { show: false } },
        legend: { show: false },
        tooltip: {
            y: {
                formatter: (value) => value.toLocaleString("en-US") + " enrollments",
            },
        },
    });
    const selected = { growth: growthOptions, completion: completionOptions, category: categoryOptions }[kind], type = { growth: 'area', completion: 'bar', category: 'radar' }[kind];
    return { ...flatChartOptions(selected, chartPalette(), type === 'area'), chart: { ...selected.chart, type }, series: ({ growth: [{ name: 'Completed', data: data.completionSeries }, { name: 'In progress', data: data.enrollmentSeries.map((value, index) => value - (data.completionSeries[index] ?? 0)) }], completion: [{ name: 'Learners', data: [data.completions, data.enrollments - data.completions] }], category: [{ name: 'Enrollments', data: LMS_CATEGORIES.map(name => data.selected.filter(course => course.category === name).reduce((sum, course) => sum + Math.round(course.students * period / 30), 0)) }] })[kind] };
}
