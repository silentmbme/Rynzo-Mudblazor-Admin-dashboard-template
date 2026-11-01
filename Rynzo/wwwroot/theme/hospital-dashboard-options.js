import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';
export function options(kind, envelope) {
    const { data, period } = envelope, base = baseChartOptions(document.documentElement.classList.contains('dark'));
    const flowOptions = mergeOptions(base, {
        colors: [token("--info"), token("--success")],
        stroke: { curve: "stepline", width: [2.5, 2], dashArray: [0, 5] },
        markers: { size: 4, strokeWidth: 2, strokeColors: token("--card"), hover: { sizeOffset: 2 } },
        grid: { borderColor: token("--border"), strokeDashArray: 2, xaxis: { lines: { show: true } }, yaxis: { lines: { show: true } } },
        dataLabels: { enabled: false },
        fill: { opacity: [0.09, 0.03] },
        xaxis: { categories: data.labels },
        yaxis: {
            min: 0,
            title: { text: "Patients", style: { color: token("--muted-foreground"), fontWeight: 400, fontSize: "11px" } },
            labels: { formatter: (value) => Math.round(value).toString() },
        },
        legend: { position: "top", horizontalAlign: "left" },
        tooltip: {
            shared: true,
            intersect: false,
            y: { formatter: (value) => `${Math.round(value)} patients` },
        },
    });
    const departmentOptions = mergeOptions(base, {
        colors: data.departments.map((item) => token(item.color)),
        stroke: { width: 0 },
        fill: { opacity: 0.95 },
        plotOptions: {
            bar: {
                horizontal: true,
                distributed: true,
                borderRadius: 4,
                barHeight: "32%",
            },
        },
        legend: { show: false },
        xaxis: {
            categories: data.departments.map((item) => item.name),
            labels: {
                formatter: (value) => Math.round(Number(value)).toString(),
            },
        },
        yaxis: { labels: { maxWidth: 125 } },
        tooltip: { y: { formatter: (value) => `${value} visits` } },
    });
    const occupancyOptions = mergeOptions(base, {
        colors: [token("--chart-1")],
        labels: ["Beds occupied"],
        stroke: { lineCap: "round" },
        plotOptions: {
            radialBar: {
                startAngle: -110,
                endAngle: 110,
                hollow: { size: "76%" },
                track: { background: token("--muted") },
                dataLabels: {
                    name: { offsetY: 25, fontSize: "12px" },
                    value: {
                        offsetY: -15,
                        fontSize: "28px",
                        fontWeight: 600,
                        formatter: (value) => value.toFixed(1) + "%",
                    },
                },
            },
        },
    });
    const selected = { flow: flowOptions, department: departmentOptions, occupancy: occupancyOptions }[kind], type = { flow: 'area', department: 'bar', occupancy: 'radialBar' }[kind];
    return { ...flatChartOptions(selected, chartPalette(), false), chart: { ...selected.chart, type }, series: ({ flow: [{ name: 'Arrivals', data: data.admissions }, { name: 'Discharged', data: data.discharged }], department: [{ name: 'Visits', data: data.departments.map(item => Math.round(item.visits * period / 7)) }], occupancy: [data.occupancy] })[kind] };
}
