import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';
export function options(kind, data) {
    const base = baseChartOptions(document.documentElement.classList.contains('dark'));
    const money = value => '$' + Number(value).toLocaleString('en-US');
    if (kind === 'sources') {
        const chart = mergeOptions(base, {
            chart: { type: 'bar' },
            colors: [token('--chart-1'), token('--chart-2'), token('--chart-3'), token('--chart-4')],
            plotOptions: { bar: { distributed: true, columnWidth: '32%', borderRadius: 5 } },
            dataLabels: { enabled: false },
            xaxis: { categories: data.sources.map(row => row[0]) },
            yaxis: { min: 0, max: 100, tickAmount: 4, labels: { formatter: value => value + '%' } },
            grid: { borderColor: token('--border'), strokeDashArray: 4 },
            legend: { show: false },
            tooltip: { y: { formatter: (value, context) => value + '% · ' + data.sources[context.dataPointIndex][2] } }
        });
        return { ...flatChartOptions(chart, chartPalette(), false), series: [{ name: 'Revenue share', data: data.sources.map(row => row[1]) }] };
    }
    if (kind === 'expenses') {
        const chart = mergeOptions(base, {
            chart: { type: 'bar' },
            colors: [token('--chart-2')],
            plotOptions: { bar: { horizontal: true, barHeight: '28%', borderRadius: 3 } },
            dataLabels: { enabled: true, textAnchor: 'start', offsetX: 8, style: { colors: [token('--foreground')], fontSize: '10px', fontWeight: 500 }, formatter: money },
            xaxis: { categories: data.expenses.map(row => row[0]), max: 45000, tickAmount: 3, labels: { formatter: value => '$' + (Number(value) / 1000).toFixed(0) + 'K' } },
            yaxis: { labels: { minWidth: 80, maxWidth: 100 } },
            grid: { borderColor: token('--border'), strokeDashArray: 4 },
            tooltip: { y: { formatter: money } },
            legend: { show: false }
        });
        return { ...flatChartOptions(chart, chartPalette(), false), series: [{ name: 'Spend', data: data.expenses.map(row => Number(row[1].replace(/[^0-9.]/g, ''))) }] };
    }
    const chart = mergeOptions(base, {
        chart: { type: 'line' },
        colors: [token('--primary'), token('--chart-2'), token('--success')],
        xaxis: { categories: ['Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug'] },
        yaxis: { labels: { formatter: value => '$' + Math.round(value) + 'K' } },
        plotOptions: { bar: { columnWidth: '25%', borderRadius: 3 } },
        stroke: { curve: 'smooth', width: [2, 0, 3] },
        markers: { size: [0, 0, 4], strokeWidth: 2 },
        fill: { opacity: [0.1, 0.8, 1] },
        grid: { borderColor: token('--border'), strokeDashArray: 4 },
        legend: { position: 'top', horizontalAlign: 'left', fontSize: '11px' },
        tooltip: { shared: true, y: { formatter: value => money(value) + 'K' } }
    });
    return { ...flatChartOptions(chart, chartPalette(), true), series: data.series.map((series, index) => ({ ...series, type: index === 0 ? 'area' : index === 1 ? 'column' : 'line' })) };
}

