import { baseChartOptions, mergeOptions, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';

const points = (count, seed = 1) => Array.from({ length: count }, (_, index) => [18 + ((index * 17 + seed * 9) % 78), 22 + ((index * 29 + seed * 7) % 72), 10 + ((index * 11) % 28)]);
const xy = values => values.map(value => ({ x: value[0], y: value[1] }));
const bubblePoints = values => values.map(value => ({ x: value[0], y: value[1], z: value[2] }));

export function options(kind, data) {
  const base = baseChartOptions(document.documentElement.classList.contains('dark'));
  const palette = chartPalette();
  const bubble = ['opportunity', 'clusters', 'risk'].includes(kind);
  const count = data.count || 184;
  let series;
  if (kind === 'campaigns') series = [['Organic', 1], ['Paid', 2], ['Referral', 3], ['Direct', 4]].map(([name, seed]) => ({ name, data: xy(points(12, seed)) }));
  else if (kind === 'regression') series = [{ name: 'Observations', data: xy(points(28, 5)) }, { name: 'Trendline', data: [{ x: 18, y: 27 }, { x: 94, y: 88 }] }];
  else if (kind === 'outliers') series = [{ name: 'Normal', data: xy(points(32, 6)) }, { name: 'Outliers', data: [{ x: 91, y: 22 }, { x: 25, y: 96 }, { x: 86, y: 92 }] }];
  else { const values = points(kind === 'density' ? Math.min(count, 120) : 18, 7); series = [{ name: kind === 'density' ? 'Observations' : 'Portfolio', data: bubble ? bubblePoints(values) : xy(values) }]; }
  const config = mergeOptions(base, { chart: { type: bubble ? 'bubble' : 'scatter', zoom: { enabled: true, type: 'xy' } }, colors: kind === 'outliers' ? [palette[0], palette[3]] : palette, dataLabels: { enabled: false }, legend: { show: ['campaigns', 'regression', 'outliers'].includes(kind), position: 'bottom', fontSize: '11px' }, markers: { size: kind === 'density' ? 5 : 7, strokeWidth: 0, fillOpacity: kind === 'density' ? .32 : 1 }, xaxis: { min: 0, max: 105, tickAmount: 5, labels: { style: { fontSize: '10px' } } }, yaxis: { min: 0, max: 105, tickAmount: 5, labels: { style: { fontSize: '10px' } } }, annotations: kind === 'quadrant' || kind === 'outliers' ? { xaxis: [{ x: 55, borderColor: palette[3], label: { text: 'Priority' } }], yaxis: [{ y: 60, borderColor: palette[3], label: { text: 'Threshold' } }] } : {}, tooltip: { shared: false, intersect: true } });
  return { ...flatChartOptions(config, palette, false), series };
}
