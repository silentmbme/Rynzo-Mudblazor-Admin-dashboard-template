import {
  baseChartOptions,
  mergeOptions,
  token,
  chartPalette,
  flatChartOptions,
} from "../vendor/apexcharts/source-options.js";
export function options(kind, envelope) {
  const visible = envelope.visible,
    statuses = ["Pending", "In transit", "Delayed", "Delivered"],
    base = baseChartOptions(
      document.documentElement.classList.contains("dark"),
    );
  const distribution = mergeOptions(base, {
    labels: statuses,
    colors: [
      token("--warning"),
      token("--primary"),
      token("--danger"),
      token("--success"),
    ],
    stroke: { width: 5, colors: [token("--card")] },
    fill: { type: "solid", opacity: 1 },
    plotOptions: {
      pie: {
        expandOnClick: false,
        donut: {
          size: "68%",
          labels: {
            show: true,
            name: {
              show: true,
              fontSize: "11px",
              fontWeight: 500,
              color: token("--muted-foreground"),
              offsetY: -4,
            },
            value: {
              show: true,
              fontSize: "20px",
              fontWeight: 700,
              color: token("--foreground"),
              offsetY: 4,
              formatter: (value) => Math.round(value),
            },
            total: {
              show: true,
              label: "SHIPMENTS",
              fontSize: "10px",
              fontWeight: 600,
              color: token("--muted-foreground"),
              formatter: (chart) =>
                chart.globals.seriesTotals.reduce(
                  (sum, value) => sum + value,
                  0,
                ),
            },
          },
        },
      },
    },
    legend: {
      show: true,
      position: "bottom",
      horizontalAlign: "center",
      fontSize: "11px",
      fontWeight: 500,
      markers: { width: 8, height: 8, radius: 2 },
      itemMargin: { horizontal: 8, vertical: 3 },
    },
    dataLabels: { enabled: false },
    tooltip: {
      y: { formatter: (value) => `${value} shipment${value === 1 ? "" : "s"}` },
    },
  });
  const loadOptions = mergeOptions(base, {
    colors: [token("--primary"), token("--chart-3")],
    stroke: { width: [0, 4], curve: "smooth", lineCap: "round" },
    fill: {
      type: ["solid", "gradient"],
      gradient: {
        shadeIntensity: 0.2,
        opacityFrom: 0.98,
        opacityTo: 0.72,
        stops: [0, 92, 100],
      },
    },
    markers: {
      size: [0, 5],
      strokeWidth: 3,
      strokeColors: token("--card"),
      hover: { sizeOffset: 2 },
    },
    plotOptions: {
      bar: {
        columnWidth: "42%",
        borderRadius: 7,
        borderRadiusApplication: "end",
      },
    },
    dataLabels: {
      enabled: true,
      enabledOnSeries: [1],
      formatter: (value) => Math.round(value) + "%",
      offsetY: -8,
      style: {
        fontSize: "11px",
        fontWeight: 600,
        colors: [token("--muted-foreground")],
      },
      background: { enabled: false },
    },
    grid: {
      borderColor: token("--border"),
      strokeDashArray: 4,
      padding: { top: 18, right: 8, left: 4, bottom: 0 },
    },
    xaxis: {
      categories: visible.map((s) => s.id.replace("SHP-", "")),
      axisBorder: { show: false },
      axisTicks: { show: false },
    },
    yaxis: [
      {
        seriesName: "Cargo weight",
        min: 0,
        labels: { formatter: (v) => Math.round(v) + " kg" },
      },
      {
        seriesName: "Route progress",
        opposite: true,
        min: 0,
        max: 100,
        labels: { formatter: (v) => Math.round(v) + "%" },
      },
    ],
    legend: {
      position: "top",
      horizontalAlign: "left",
      fontSize: "12px",
      markers: { width: 9, height: 9, radius: 3 },
    },
    tooltip: {
      shared: true,
      intersect: false,
      y: {
        formatter: (value, { seriesIndex }) =>
          seriesIndex === 0
            ? Math.round(value) + " kg"
            : Math.round(value) + "% complete",
      },
    },
  });
  const selected = kind === "cargo" ? loadOptions : distribution;
  return {
    ...flatChartOptions(selected, chartPalette(), false),
    chart: { ...selected.chart, type: kind === "cargo" ? "line" : "donut" },
    series:
      kind === "cargo"
        ? [
            {
              name: "Cargo weight",
              type: "column",
              data: visible.map((s) => s.weight),
            },
            {
              name: "Route progress",
              type: "line",
              data: visible.map((s) => s.progress),
            },
          ]
        : statuses.map(
            (status) => visible.filter((s) => s.status === status).length,
          ),
  };
}
