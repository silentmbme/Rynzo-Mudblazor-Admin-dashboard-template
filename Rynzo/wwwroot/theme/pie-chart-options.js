import { baseChartOptions, mergeOptions, chartPalette, token } from '../vendor/apexcharts/source-options.js';
const sum=values=>values.reduce((a,b)=>a+b,0);
const money=value=>new Intl.NumberFormat('en-US',{style:'currency',currency:'USD',maximumFractionDigits:0}).format(value);
const compact=value=>Math.abs(value)>=1000000?`$${(value/1000000).toFixed(2)}m`:Math.abs(value)>=1000?`$${(value/1000).toFixed(1)}k`:money(value);
const format=(value,unit)=>unit==='USD'?money(value):unit==='hours / week'?`${value.toFixed(1)}h`:value.toLocaleString('en-US');
const escape=value=>String(value).replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));

export function options(kind,data){
    const comparison=kind.startsWith('comparison-'),regional=/^region[0-3]$/.test(kind);
    const group=data.groups.find(g=>g.kind===(comparison?'revenue':kind));
    if(!group)throw new Error(`Unsupported pie chart kind: ${kind}`);
    const values=comparison&&kind==='comparison-baseline'?(data.baseline==='Year ago'?group.yearAgo:group.previous):group.values;
    const hiddenPrefix=regional?'fulfillment':kind;
    const active=values.map((_,i)=>i).filter(i=>comparison||!data.hidden.includes(hiddenPrefix+i));
    const visible=active.map(i=>values[i]),total=sum(values),visibleTotal=sum(visible),palette=chartPalette();
    const unit=group.unit;
    const display=value=>unit==='USD'?compact(value):unit==='hours / week'?`${value.toFixed(0)}h`:value.toLocaleString('en-US');
    const pie=['leads','devices','campaigns'].includes(kind);
    const base=mergeOptions(baseChartOptions(document.documentElement.classList.contains('dark')),{
        chart:{type:pie?'pie':'donut',animations:{enabled:!window.matchMedia('(prefers-reduced-motion: reduce)').matches,speed:550},toolbar:{show:false}},
        series:visible.length?visible:[0],labels:active.length?active.map(i=>group.labels[i]):['No segments selected'],colors:active.length?active.map(i=>palette[i]):[token('--muted')],
        legend:{show:comparison,position:'bottom',horizontalAlign:'center',fontSize:'10px',itemMargin:{horizontal:8,vertical:4}},
        stroke:{width:3,colors:[token('--card')]},
        dataLabels:{enabled:pie,formatter:value=>value>=7?`${value.toFixed(0)}%`:'',style:{fontSize:'10px',fontWeight:600},dropShadow:{enabled:false}},
        plotOptions:{pie:{expandOnClick:true,customScale:.96,dataLabels:{offset:-6,minAngleToShowLabel:12},donut:{size:'70%',labels:{show:!pie,name:{show:true,fontSize:regional?'9px':'11px',offsetY:20,color:token('--muted-foreground')},value:{show:true,fontSize:regional?'20px':'27px',fontWeight:600,offsetY:-13,color:token('--foreground'),formatter:v=>display(Number(v))},total:{show:true,showAlways:true,label:comparison?'Period total':regional?'Visible orders':active.length<values.length?'Visible total':unit==='USD'?'Revenue · USD':kind==='customers'?'Active customers':kind==='time'?'Weekly capacity':'Total',fontSize:regional?'9px':'11px',color:token('--muted-foreground'),formatter:()=>display(comparison?total:visibleTotal)}}}}},
        tooltip:{shared:false,intersect:true,custom:({seriesIndex})=>{
            const i=active[seriesIndex];if(i===undefined)return '';
            return `<div class="pie-tooltip"><span>${escape(group.labels[i])}</span><br/><b>${format(values[i],unit)}</b><br/>${(values[i]/total*100).toFixed(1)}% of full dataset${visibleTotal!==total?`<br/>${(values[i]/visibleTotal*100).toFixed(1)}% of visible selection`:''}</div>`;
        }},
        grid:{padding:{top:8,bottom:8,left:8,right:8}},
        responsive:[{breakpoint:600,options:{legend:{fontSize:'9px',itemMargin:{horizontal:5,vertical:3}},dataLabels:{style:{fontSize:'9px'}}}}]
    });
    let style;
    switch(kind){
        case 'revenue':style={plotOptions:{pie:{donut:{size:'73%'}}},fill:{type:'gradient',gradient:{type:'diagonal1',shadeIntensity:.1,opacityFrom:1,opacityTo:.82,stops:[0,100]}}};break;
        case 'leads':style={plotOptions:{pie:{startAngle:25,endAngle:385}},stroke:{width:2}};break;
        case 'customers':style={plotOptions:{pie:{donut:{size:'83%'}}},stroke:{width:4}};break;
        case 'budget':style={plotOptions:{pie:{startAngle:-90,endAngle:90,donut:{size:'70%',labels:{total:{label:active.length<values.length?'Visible spend':'Operating spend'},name:{offsetY:20},value:{offsetY:-13}}}}},dataLabels:{enabled:false},grid:{padding:{top:10,bottom:0,left:4,right:4}}};break;
        case 'devices':style={plotOptions:{pie:{startAngle:-45,endAngle:315}},stroke:{width:5},dataLabels:{style:{fontSize:'12px'}}};break;
        case 'renewal':style={plotOptions:{pie:{donut:{size:'86%',labels:{total:{label:active.length<values.length?'Visible contracts':'Contracts due'}}}}},stroke:{width:3}};break;
        case 'campaigns':style={plotOptions:{pie:{startAngle:90,endAngle:450}},stroke:{width:5},fill:{type:'gradient',gradient:{shadeIntensity:.12,opacityFrom:1,opacityTo:.8,stops:[0,100]}}};break;
        case 'time':style={plotOptions:{pie:{startAngle:-135,endAngle:135,donut:{size:'63%'}}},stroke:{width:5}};break;
        case 'comparison-current':case 'comparison-baseline':style={plotOptions:{pie:{donut:{size:'77%'}}},stroke:{width:3}};break;
        default:if(!regional)throw new Error(`Missing pie chart configuration: ${kind}`);style={plotOptions:{pie:{donut:{size:'76%'}}},stroke:{width:3},grid:{padding:{top:4,bottom:4,left:4,right:4}}};
    }
    return mergeOptions(base,style);
}

export function exportCsv(data,period,category){
    const rows=[['Period','Category','Chart','Segment','Current value','Current share %','Previous value','Previous share %','Year ago value','Year ago share %','Unit','Visible']];
    for(const g of data.groups){
        if(category!=='All Charts'&&category!==g.category)continue;
        const total=sum(g.values),previous=sum(g.previous),annual=sum(g.yearAgo),prefix=g.kind.startsWith('region')?'fulfillment':g.kind;
        g.labels.forEach((label,i)=>rows.push([period,g.category,g.kind,label,g.values[i],g.values[i]/total*100,g.previous[i],g.previous[i]/previous*100,g.yearAgo[i],g.yearAgo[i]/annual*100,g.unit,!data.hidden.includes(prefix+i)]));
        rows.push([period,g.category,g.kind,'TOTAL',total,100,previous,100,annual,100,g.unit,'']);
    }
    const csv=rows.map(row=>row.map(value=>`"${String(value).replaceAll('"','""')}"`).join(',')).join('\r\n');
    const url=URL.createObjectURL(new Blob(['\ufeff'+csv],{type:'text/csv;charset=utf-8'})),link=document.createElement('a');link.href=url;link.download=`rynzo-pie-${period.replaceAll(' ','-').toLowerCase()}.csv`;document.body.append(link);link.click();link.remove();setTimeout(()=>URL.revokeObjectURL(url),1000);
}

