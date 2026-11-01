import { baseChartOptions, mergeOptions, token, chartPalette, flatChartOptions } from '../vendor/apexcharts/source-options.js';
export function options(kind, data) {
    const money=n=>new Intl.NumberFormat('en-US',{style:'currency',currency:'USD',maximumFractionDigits:2}).format(n);
    const base=baseChartOptions(document.documentElement.classList.contains('dark'));
    const allocation=kind==='allocation';
    const selected=allocation?mergeOptions(base,{
        labels:data.assets.map(a=>a.symbol),colors:data.assets.map(a=>token(a.color)),
        stroke:{width:3,colors:[token('--card')]},dataLabels:{enabled:false},legend:{position:'bottom'},
        plotOptions:{pie:{donut:{size:'75%',labels:{show:true,total:{show:true,showAlways:true,label:'Portfolio',formatter:()=>money(data.holdings)}}}}},
        tooltip:{y:{formatter:money}}
    }):mergeOptions(base,{
        colors:[token('--chart-1')],stroke:{curve:'smooth',width:2},fill:{opacity:.18,type:'solid'},
        xaxis:{type:'datetime'},yaxis:{labels:{formatter:money}},tooltip:{x:{format:'MMM dd HH:mm'},y:{formatter:money}}
    });
    return {...flatChartOptions(selected,chartPalette(),!allocation),chart:{...selected.chart,type:allocation?'donut':'area'},series:allocation?data.assets.map(a=>a.holding*a.price):[{name:data.asset.symbol,data:data.history.prices}]};
}
