import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';
export function cryptoMoney(value) {
    return new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
    }).format(value);
}
export function cryptoCompact(value) {
    return new Intl.NumberFormat("en-US", { notation: "compact", maximumFractionDigits: 1 }).format(value);
}
export function options(kind, envelope) {
    const { asset, range, history, holdings } = envelope, CRYPTO_ASSETS = envelope.assets, base = baseChartOptions(document.documentElement.classList.contains('dark'));
    const priceOptions = mergeOptions(base, {
        chart: { type: "area" },
        colors: [token("--chart-1")],
        stroke: { curve: "smooth", width: 3 },
        fill: { opacity: 0.18, type: "solid" },
        xaxis: {
            type: "datetime",
            tickAmount: 5,
            labels: {
                datetimeUTC: true,
                format: range === "24h" ? "HH:mm" : "MMM dd",
            },
        },
        yaxis: {
            min: Math.min(...history.prices.map((point) => point.y)) * 0.992,
            max: Math.max(...history.prices.map((point) => point.y)) * 1.008,
            labels: { formatter: (value) => "$" + cryptoCompact(value) },
        },
        tooltip: { x: { format: "MMM dd, HH:mm" }, y: { formatter: cryptoMoney } },
    });
    const volumeOptions = mergeOptions(base, {
        colors: [token("--chart-2")],
        stroke: { width: 0 },
        plotOptions: { bar: { columnWidth: "45%", borderRadius: 3 } },
        xaxis: {
            type: "datetime",
            tickAmount: 5,
            labels: {
                datetimeUTC: true,
                format: range === "24h" ? "HH:mm" : "MMM dd",
            },
        },
        yaxis: {
            labels: {
                formatter: (value) => "$" + cryptoCompact(value) + "M",
            },
        },
        tooltip: {
            x: { format: "MMM dd, HH:mm" },
            y: { formatter: (value) => cryptoMoney(value * 1000000) },
        },
    });
    const allocationOptions = mergeOptions(base, {
        labels: CRYPTO_ASSETS.map((item) => item.symbol),
        colors: CRYPTO_ASSETS.map((item) => token(item.color)),
        legend: { show: false },
        stroke: { width: 4, colors: [token("--card")] },
        plotOptions: {
            pie: {
                donut: {
                    size: "78%",
                    labels: {
                        show: true,
                        value: { formatter: (value) => cryptoMoney(Number(value)) },
                        total: {
                            show: true,
                            showAlways: true,
                            label: "Total assets",
                            formatter: () => cryptoMoney(holdings),
                        },
                    },
                },
            },
        },
        tooltip: { y: { formatter: cryptoMoney } },
    });
    const selected = { price: priceOptions, volume: volumeOptions, allocation: allocationOptions }[kind], type = { price: 'area', volume: 'bar', allocation: 'donut' }[kind];
    return { ...flatChartOptions(selected, chartPalette(), type === 'area'), chart: { ...selected.chart, type }, series: ({ price: [{ name: asset.symbol, data: history.prices }], volume: [{ name: 'Volume', data: history.volume }], allocation: CRYPTO_ASSETS.map(item => item.price * item.holding) })[kind] };
}
