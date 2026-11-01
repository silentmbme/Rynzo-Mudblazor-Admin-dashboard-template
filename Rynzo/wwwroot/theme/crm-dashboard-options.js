import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';
export function options(kind, data) {
    const base = baseChartOptions(document.documentElement.classList.contains('dark'));
    if (kind === 'conversations') {
        const linked = data.conversations.map(signal => ({ signal, deal: data.priorityDeals.find(deal => deal.company === signal[0]) })).filter(item => item.deal);
        const values = linked.map(({ deal }) => Number(deal.value.replace(/[$K,]/g, '')));
        const total = values.reduce((sum, value) => sum + value, 0);
        const chart = mergeOptions(base, {
            chart: { type: 'line', stacked: false },
            colors: [token('--primary'), token('--info')],
            plotOptions: { bar: { columnWidth: '30%', borderRadius: 4 } },
            stroke: { curve: 'smooth', width: [0, 3] },
            fill: { opacity: [1, 0.12], type: 'solid' },
            markers: { size: [0, 4], strokeWidth: 2, strokeColors: token('--card') },
            legend: { show: true, position: 'top', horizontalAlign: 'left', fontSize: '10px' },
            dataLabels: { enabled: false },
            xaxis: { categories: linked.map(({ deal }) => deal.company), labels: { style: { fontSize: '10px' }, trim: true, hideOverlappingLabels: false } },
            yaxis: [
                { seriesName: 'Deal value', min: 0, labels: { formatter: value => '$' + Math.round(value) + 'K' } },
                { seriesName: 'Pipeline share', opposite: true, min: 0, max: 100, tickAmount: 4, labels: { formatter: value => Math.round(value) + '%' } },
            ],
            tooltip: { shared: true, intersect: false, y: [
                { formatter: value => '$' + value + 'K opportunity value' },
                { formatter: value => value.toFixed(1) + '% of linked pipeline' },
            ] },
            noData: { text: 'No linked opportunities' },
        });
        return { ...flatChartOptions(chart, chartPalette(), true), series: [
            { name: 'Deal value', type: 'column', data: values },
            { name: 'Pipeline share', type: 'area', data: values.map(value => total ? value / total * 100 : 0) },
        ] };
    }

    const options = mergeOptions(base, {
        chart: {
            type: "bubble",
            toolbar: {
                show: false,
            },
        },
        colors: [token("--primary"), token("--info"), token("--warning")],
        fill: { opacity: 0.65 },
        stroke: { width: 1, colors: [token("--card")] },
        xaxis: {
            min: 0,
            max: 100,
            tickAmount: 5,
            title: {
                text: "Close confidence",
            },
            labels: {
                formatter: (value) => `${value}%`,
            },
        },
        yaxis: {
            min: 0,
            max: 150,
            tickAmount: 3,
            title: {
                text: "Deal value ($K)",
            },
            labels: {
                formatter: (value) => `$${value}K`,
            },
        },
        grid: {
            borderColor: token("--border"),
            strokeDashArray: 3,
            xaxis: { lines: { show: true } },
        },
        dataLabels: {
            enabled: false,
        },
        legend: {
            position: "bottom",
            horizontalAlign: "center",
            fontSize: "11px",
        },
        tooltip: {
            x: {
                formatter: (value) => `${value}% confidence`,
            },
            y: {
                formatter: (value) => `$${value}K value`,
            },
        },
    });
    return { ...flatChartOptions(options, chartPalette(), false), series: data.mapSeries };
}
