import ApexCharts from '../vendor/apexcharts/apexcharts.esm.js';
const instances=new Map();
export async function render(element,optionsModule,kind,data,height){
 dispose(element);const factory=await import(optionsModule);const options=factory.options(kind,data);
 const chart=new ApexCharts(element,{...options,chart:{...options.chart,height}});instances.set(element,chart);
 try{await chart.render();}catch(error){dispose(element);throw error;}
}
export function dispose(element){const chart=instances.get(element);if(chart){chart.destroy();instances.delete(element);}}
export function snapshot(){return [...instances.values()].map(c=>({type:c.w.config.chart.type,theme:c.w.config.theme?.mode,series:c.w.config.series,hidden:c.w.globals.collapsedSeriesIndices.length}));}
