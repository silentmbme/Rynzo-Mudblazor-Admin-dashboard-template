import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';
export function options(kind, data) {
    const money = value => Number(value).toLocaleString('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 });
    const chart = mergeOptions(baseChartOptions(document.documentElement.classList.contains('dark')), {
        chart: { type: 'bar' },
        colors: [token('--primary'), token('--success')],
        plotOptions: { bar: { columnWidth: '45%', borderRadius: 3 } },
        dataLabels: { enabled: false },
        xaxis: { categories: data.dates, labels: { rotate: -35, hideOverlappingLabels: true } },
        yaxis: { min: 0, labels: { formatter: money } },
        grid: { borderColor: token('--border'), strokeDashArray: 4 },
        legend: { position: 'top', horizontalAlign: 'left', fontSize: '11px' },
        tooltip: { shared: true, intersect: false, y: { formatter: money } }
    });
    return { ...flatChartOptions(chart, chartPalette(), false), series: data.series };
}
