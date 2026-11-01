import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';

export function options(kind, data) {
    const base = baseChartOptions(document.documentElement.classList.contains('dark'));
    const palette = chartPalette();
    let selected;
    if (kind === 'visitors') {
        selected = mergeOptions(base, {
            chart: { type: 'line' }, colors: [palette[0], palette[1]],
            xaxis: { categories: data.days, labels: { rotate: 0, style: { fontSize: '10px' } } },
            yaxis: { tickAmount: 4, labels: { formatter: value => `${Math.round(value)}k`, style: { fontSize: '10px' } } },
            stroke: { curve: 'smooth', width: [0, 2.5] },
            fill: { opacity: [.8, 1] },
            plotOptions: { bar: { columnWidth: '35%', borderRadius: 3, borderRadiusApplication: 'end' } },
            markers: { size: [0, 3], strokeWidth: 2, strokeColors: token('--card'), hover: { size: 5 } },
            grid: { padding: { left: 0, right: 12, top: 8, bottom: 0 } },
            legend: { position: 'top', horizontalAlign: 'left', fontSize: '11px', offsetX: 8 },
            tooltip: { shared: true, intersect: false, y: { formatter: value => `${value}k` } }
        });
    } else if (kind === 'device') {
        selected = mergeOptions(base, {
            chart: { type: 'radar' }, colors: [palette[0], palette[1]],
            xaxis: { categories: ['Engagement', 'Pages / visit', 'Conversion', 'Speed', 'Retention'], labels: { style: { fontSize: '10px', colors: Array(5).fill(token('--muted-foreground')) } } },
            yaxis: { min: 0, max: 100, tickAmount: 4, show: false },
            stroke: { width: 2 }, fill: { opacity: .1 }, markers: { size: 3, strokeWidth: 0 },
            plotOptions: { radar: { polygons: { strokeColors: token('--border'), connectorColors: token('--border'), fill: { colors: ['transparent'] } } } },
            legend: { position: 'bottom', horizontalAlign: 'center', fontSize: '11px' }
        });
    } else if (kind === 'conversion') {
        selected = mergeOptions(base, {
            chart: { type: 'bar' }, colors: [palette[0], palette[1], palette[3], palette[4]],
            plotOptions: { bar: { horizontal: true, distributed: true, borderRadius: 4, barHeight: '52%', dataLabels: { position: 'top' } } },
            fill: { opacity: .18 }, stroke: { width: 0 },
            xaxis: { categories: ['Product page', 'Pricing', 'Start trial', 'Activation'], min: 0, max: 100, tickAmount: 4, labels: { style: { fontSize: '10px' }, formatter: value => `${Math.round(value)}%` } },
            yaxis: { labels: { style: { fontSize: '11px' } } },
            dataLabels: { enabled: true, textAnchor: 'end', offsetX: -8, formatter: value => `${value}%`, style: { fontSize: '11px', fontWeight: 600, colors: [token('--foreground')] }, background: { enabled: false }, dropShadow: { enabled: false } },
            legend: { show: false }, grid: { padding: { left: 0, right: 8, top: 0, bottom: 0 } },
            tooltip: { y: { formatter: value => `${value}%` } }
        });
    } else throw new Error(`Unknown Analytics chart: ${kind}`);
    // Blazor serializes optional series types as null; ApexCharts expects the key
    // to be absent for a series that inherits the chart's type.
    const series = data[kind + 'Series'].map(({ type, ...item }) => type ? { ...item, type } : item);
    return { ...flatChartOptions(selected, palette, false), series };
}
