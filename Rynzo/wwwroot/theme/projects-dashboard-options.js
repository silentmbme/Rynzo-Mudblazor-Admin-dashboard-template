import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';
export function options(kind, data) {
    const base = baseChartOptions(document.documentElement.classList.contains('dark'));
    const delivery = mergeOptions(base, {
        chart: { type: 'line', stacked: false },
        colors: [token('--primary'), token('--success'), token('--destructive')],
        xaxis: { categories: ['Aug 05', 'Aug 12', 'Aug 19', 'Aug 26', 'Sep 02', 'Sep 09'] },
        yaxis: { min: 0, labels: { formatter: value => Math.round(value).toString() } },
        plotOptions: { bar: { columnWidth: '32%', borderRadius: 4 } },
        stroke: { curve: 'smooth', width: [0, 3, 2], dashArray: [0, 0, 4] },
        fill: { opacity: [1, 0.12, 1] },
        markers: { size: 0, hover: { sizeOffset: 4 } },
        legend: { position: 'top', horizontalAlign: 'left', fontSize: '10px' },
        tooltip: { shared: true, intersect: false, y: { formatter: value => Math.round(value) + ' tasks' } },
    });
    const budget = mergeOptions(base, {
        chart: { type: 'bar', stacked: true },
        colors: [token('--primary'), token('--muted')],
        plotOptions: { bar: { horizontal: true, barHeight: '32%', borderRadius: 3 } },
        xaxis: { categories: ['Website', 'Mobile app', 'CRM', 'Automation'], max: 150, labels: { formatter: value => '$' + Math.round(Number(value)) + 'K' } },
        yaxis: { labels: { style: { fontSize: '11px' } } },
        dataLabels: { enabled: false },
        legend: { position: 'top', horizontalAlign: 'left', fontSize: '10px' },
        tooltip: { y: { formatter: value => '$' + value + 'K' } },
    });
    const team = mergeOptions(base, {
        chart: { type: 'bar' },
        colors: [token('--info'), token('--primary'), token('--chart-3'), token('--success'), token('--warning')],
        plotOptions: { bar: { horizontal: true, distributed: true, barHeight: '34%', borderRadius: 3 } },
        xaxis: { categories: ['Design', 'Development', 'QA', 'Product', 'Operations'], min: 0, max: 100, tickAmount: 4, labels: { formatter: value => Math.round(Number(value)) + '%' } },
        yaxis: { labels: { style: { fontSize: '11px' } } },
        dataLabels: { enabled: false },
        legend: { show: false },
        tooltip: { y: { formatter: value => value + '% on time' } },
    });
    const selected = { delivery, budget, team }[kind];
    const series = kind === 'delivery' ? data.chartSeries.map(series => ({ ...series, type: series.name === 'Completed' ? 'area' : series.type })) : data[kind === 'budget' ? 'budgetChartSeries' : 'teamPerformanceChartSeries'].map(({ name, data }) => ({ name, data }));
    return { ...flatChartOptions(selected, chartPalette(), kind === 'delivery'), series };
}
