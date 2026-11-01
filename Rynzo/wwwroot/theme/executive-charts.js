import ApexCharts from '../vendor/apexcharts/apexcharts.esm.js';
import { baseChartOptions, mergeOptions, token, chartPalette, sparklineOptions, flatChartOptions } from '../vendor/apexcharts/source-options.js';

const charts = new Map();
const currency = value => Number(value).toLocaleString('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 });

export async function render(element, kind, data, index, requestedHeight) {
    dispose(element);
    const dark = document.documentElement.classList.contains('dark');
    const base = baseChartOptions(dark);
    const palette = dark
        ? [token('--primary'), '#51d1bc', '#ffd071', '#ff9aae', '#c6b4ff', '#a8c7ff']
        : [token('--primary'), '#12a594', '#e7a136', '#ef6b87', '#8b78df', '#5598df'];
    const label = token('--muted-foreground');
    let options, series, type;
    const height = requestedHeight > 0 ? requestedHeight : kind === 'spark' ? 52 : 300;

    if (kind === 'spark') {
        const kpi = data.executiveKpis[index];
        const color = palette[index] ?? palette[0];
        options = sparklineOptions(color);
        series = [{ name: kpi.label, data: kpi.spark }];
        type = 'area';
    } else if (kind === 'revenue') {
        type = 'line';
        series = data.revenueSeries.map((item, i) => ({ ...item, type: i === 0 ? 'area' : 'line' }));
        options = mergeOptions(base, {
            colors: [palette[0], palette[1]],
            chart: { type, stacked: false },
            stroke: { width: [2.5, 2], curve: 'smooth', dashArray: [0, 5] },
            fill: { type: 'solid', opacity: [.1, 1] },
            markers: { size: 0, hover: { size: 4 } },
            grid: { strokeDashArray: 4, padding: { left: 0, right: 10, top: 0, bottom: 0 } },
            xaxis: { categories: data.months, labels: { rotate: 0, style: { fontSize: '10px' } } },
            yaxis: { tickAmount: 4, labels: { style: { fontSize: '10px' }, formatter: value => `$${Math.round(value)}k` } },
            legend: { position: 'bottom', horizontalAlign: 'left', fontSize: '11px', offsetX: 8, itemMargin: { horizontal: 12, vertical: 4 } },
            tooltip: { shared: true, intersect: false, y: { formatter: value => currency(value * 1000) } }
        });
    } else if (kind === 'profit') {
        type = 'bar';
        series = data.profitSeries;
        options = mergeOptions(base, {
            chart: { type }, colors: [palette[0]],
            plotOptions: { bar: { columnWidth: '45%', borderRadius: 3, borderRadiusApplication: 'end' } },
            fill: { opacity: .85 }, stroke: { width: 0 },
            grid: { padding: { left: 0, right: 0, top: 0, bottom: 0 } },
            xaxis: { categories: data.months, labels: { rotate: 0, style: { fontSize: '9px' } } },
            yaxis: { min: 0, max: 100, tickAmount: 4, labels: { style: { fontSize: '10px' }, formatter: value => `${Math.round(value)}%` } },
            legend: { show: false }, tooltip: { y: { formatter: value => `${value}% gross margin` } }
        });
    } else {
        type = 'donut';
        series = data.salesByRegion.data;
        options = mergeOptions(base, {
            chart: { type }, labels: data.salesByRegion.labels,
            colors: [palette[0], palette[1], palette[3], palette[4], palette[5]],
            stroke: { width: 4, colors: [token('--card')] },
            dataLabels: { enabled: false },
            legend: { position: 'bottom', horizontalAlign: 'center', fontSize: '10px', itemMargin: { horizontal: 6, vertical: 4 }, markers: { size: 4 } },
            plotOptions: { pie: { donut: { size: '78%', labels: {
                show: true,
                name: { fontSize: '11px', color: label, offsetY: 20 },
                value: { fontSize: '24px', fontWeight: 700, color: token('--foreground'), offsetY: -14, formatter: value => `$${(Number(value) / 1000).toFixed(1)}k` },
                total: { show: true, showAlways: true, label: 'Total sales', fontSize: '11px', color: label,
                    formatter: () => `$${(data.salesByRegion.data.reduce((a, b) => a + b, 0) / 1000).toFixed(1)}k` }
            } } } },
            tooltip: { y: { formatter: currency } }
        });
    }
    const styled = flatChartOptions(options, palette, kind === 'spark');
    const chart = new ApexCharts(element, { ...styled, chart: { ...styled.chart, type, height }, series });
    charts.set(element, chart);
    try { await chart.render(); }
    catch (error) { dispose(element); throw error; }
}

export function dispose(element) {
    const chart = charts.get(element);
    if (chart) { chart.destroy(); charts.delete(element); }
}

export function snapshot() {
    return [...charts.values()].map(chart => ({ type: chart.w.config.chart.type, hidden: chart.w.globals.collapsedSeriesIndices.length,
        theme: chart.w.config.theme?.mode, spark: !!chart.w.config.chart.sparkline?.enabled, series: chart.w.config.series }));
}
