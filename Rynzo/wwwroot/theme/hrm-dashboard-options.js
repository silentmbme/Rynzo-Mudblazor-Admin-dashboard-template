import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';
const money = value => new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 }).format(value);
export function options(kind, envelope) {
    const { selected, chartDepartments } = envelope, base = baseChartOptions(document.documentElement.classList.contains('dark'));
    if (kind === 'arrangements') {
        const chart = mergeOptions(base, {
            chart: { type: 'donut' },
            labels: ['Office', 'Remote', 'On leave'],
            colors: [token('--success'), token('--primary'), token('--warning')],
            stroke: { width: 5, colors: [token('--card')] },
            plotOptions: { pie: { donut: { size: '73%', labels: { show: true,
                name: { offsetY: 20, fontSize: '11px' },
                value: { offsetY: -12, fontSize: '28px', fontWeight: 600 },
                total: { show: true, showAlways: true, label: 'Employees', color: token('--muted-foreground'), formatter: () => String(selected.length) },
            } } } },
            legend: { show: false },
            dataLabels: { enabled: false },
            tooltip: { y: { formatter: value => value + ' employees' } },
            noData: { text: 'No employees in this department' },
        });
        return { ...flatChartOptions(chart, chartPalette(), false), series: ['Present', 'Remote', 'On leave'].map(status => selected.filter(employee => employee.status === status).length) };
    }
    const attendanceOptions = mergeOptions(base, {
        chart: { type: 'bar', stacked: false },
        colors: [token("--success"), token("--primary"), token("--warning")],
        stroke: { width: 0 },
        fill: { opacity: 1 },
        plotOptions: { bar: { horizontal: true, barHeight: '64%', borderRadius: 3, borderRadiusApplication: 'end' } },
        dataLabels: { enabled: true, formatter: value => value === 0 ? '' : String(value), style: { fontSize: '11px', fontWeight: 600 }, background: { enabled: false } },
        xaxis: {
            categories: chartDepartments,
            min: 0,
            max: Math.max(1, ...chartDepartments.map(d => selected.filter(e => e.department === d).length)) + 1,
            tickAmount: Math.min(6, Math.max(1, ...chartDepartments.map(d => selected.filter(e => e.department === d).length)) + 1),
            labels: { formatter: value => Number.isInteger(value) ? String(value) : '', style: { fontSize: '10px' } },
        },
        yaxis: {
            labels: { maxWidth: 105, style: { fontSize: '11px' } },
        },
        legend: { position: "top", horizontalAlign: "left" },
        tooltip: {
            shared: true,
            intersect: false,
            y: { formatter: value => `${value} ${value === 1 ? 'employee' : 'employees'}` },
        },
    });
    const departmentOptions = mergeOptions(base, {
        colors: [token("--primary"), token("--info")],
        stroke: { width: [0, 3], curve: "smooth" },
        markers: { size: [0, 5], strokeWidth: 2 },
        plotOptions: { bar: { columnWidth: "32%", borderRadius: 5 } },
        fill: { opacity: [1, 0.1] },
        xaxis: {
            categories: chartDepartments,
            labels: { rotate: 0, style: { fontSize: "10px" } },
        },
        yaxis: [
            {
                seriesName: "Base salaries",
                min: 0,
                labels: {
                    formatter: (value) => "$" + (value / 1000).toFixed(0) + "k",
                },
            },
            {
                seriesName: "Employees",
                opposite: true,
                min: 0,
                max: Math.max(selected.length, 1),
                tickAmount: Math.max(selected.length, 1),
                labels: { formatter: (value) => Math.round(value).toString() },
            },
        ],
        legend: { position: "top", horizontalAlign: "left" },
        tooltip: {
            shared: true,
            intersect: false,
            y: [
                { formatter: (value) => money(value) },
                { formatter: (value) => value + " employees" },
            ],
        },
    });
    const selectedOptions = kind === 'attendance' ? attendanceOptions : departmentOptions;
    return { ...flatChartOptions(selectedOptions, chartPalette(), false), chart: { ...selectedOptions.chart, type: 'bar' }, series: kind === 'attendance' ? [{ name: 'On site', status: 'Present' }, { name: 'Remote', status: 'Remote' }, { name: 'On leave', status: 'On leave' }].map(item => ({ name: item.name, data: chartDepartments.map(d => selected.filter(e => e.department === d && e.status === item.status).length) })) : [{ name: 'Base salaries', type: 'column', data: chartDepartments.map(d => selected.filter(e => e.department === d).reduce((sum, e) => sum + e.salary, 0)) }, { name: 'Employees', type: 'area', data: chartDepartments.map(d => selected.filter(e => e.department === d).length) }] };
}
