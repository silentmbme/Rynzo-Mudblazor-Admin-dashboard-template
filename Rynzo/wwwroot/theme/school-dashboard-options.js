import {
  baseChartOptions,
  mergeOptions,
  token,
  chartPalette,
  flatChartOptions,
} from "../vendor/apexcharts/source-options.js";
export function options(kind, envelope) {
  const { selected, classes } = envelope;
  const base = baseChartOptions(
    document.documentElement.classList.contains("dark"),
  );
  if (kind === "attendance") {
    const chart = mergeOptions(base, {
      chart: { type: "area", zoom: { enabled: false } },
      colors: ["#b9e7d5", token("--warning")],
      stroke: {
        width: [2.5, 3],
        curve: ["straight", "smooth"],
        lineCap: "round",
        colors: ["#54a98c", token("--warning")],
      },
      fill: {
        type: ["gradient", "solid"],
        gradient: {
          shadeIntensity: 0,
          opacityFrom: 0.76,
          opacityTo: 0.08,
          stops: [0, 78, 100],
        },
        opacity: [1, 1],
      },
      markers: {
        size: [4, 5],
        shape: "circle",
        strokeWidth: 2.5,
        strokeColors: token("--card"),
        hover: { sizeOffset: 3 },
      },
      xaxis: {
        categories: classes,
        labels: { style: { fontSize: "10px", fontWeight: 500 } },
        axisBorder: { show: false },
        axisTicks: { show: false },
      },
      yaxis: {
        min: 0,
        tickAmount: 4,
        labels: {
          formatter: (value) => Math.round(Number(value)).toString(),
          style: { fontSize: "10px" },
        },
      },
      grid: {
        borderColor: token("--border"),
        strokeDashArray: 4,
        padding: { top: 4, right: 12, bottom: -2, left: 2 },
      },
      legend: {
        show: true,
        position: "top",
        horizontalAlign: "left",
        fontSize: "11px",
        fontWeight: 500,
        markers: { width: 8, height: 8, radius: 2 },
      },
      dataLabels: {
        enabled: true,
        enabledOnSeries: [1],
        formatter: (value) => value || "",
        offsetY: -7,
        style: {
          fontSize: "10px",
          fontWeight: 600,
          colors: [token("--foreground")],
        },
        background: { enabled: false },
      },
      tooltip: {
        shared: true,
        intersect: false,
        y: {
          formatter: (value) => `${value} student${value === 1 ? "" : "s"}`,
        },
      },
    });
    return {
      ...flatChartOptions(chart, chartPalette(), false),
      series: ["Present", "Absent"].map((status, index) => ({
        name: status,
        type: index === 0 ? "area" : "line",
        data: classes.map(
          (grade) =>
            selected.filter(
              (student) =>
                student.grade === grade && student.attendance === status,
            ).length,
        ),
      })),
    };
  }
  const chart = mergeOptions(base, {
    chart: { type: "bar", stacked: true },
    colors: [token("--primary"), token("--warning")],
    stroke: { width: 0 },
    plotOptions: { bar: { columnWidth: "32%", borderRadius: 3 } },
    xaxis: { categories: classes, labels: { style: { fontSize: "10px" } } },
    yaxis: {
      min: 0,
      labels: { formatter: (value) => "$" + (value / 1000).toFixed(1) + "K" },
    },
    legend: { position: "top", horizontalAlign: "left", fontSize: "10px" },
    dataLabels: { enabled: false },
    tooltip: {
      shared: true,
      intersect: false,
      y: { formatter: (value) => "$" + value.toLocaleString("en-US") },
    },
  });
  return {
    ...flatChartOptions(chart, chartPalette(), false),
    series: [
      {
        name: "Collected",
        data: classes.map((g) =>
          selected
            .filter((s) => s.grade === g)
            .reduce((sum, s) => sum + s.paid, 0),
        ),
      },
      {
        name: "Outstanding",
        data: classes.map((g) =>
          selected
            .filter((s) => s.grade === g)
            .reduce((sum, s) => sum + s.fee - s.paid, 0),
        ),
      },
    ],
  };
}
