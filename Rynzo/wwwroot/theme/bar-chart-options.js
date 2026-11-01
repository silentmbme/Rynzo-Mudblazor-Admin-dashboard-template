import { baseChartOptions, mergeOptions, chartPalette, token } from '../vendor/apexcharts/source-options.js';
const sum=values=>values.reduce((a,b)=>a+b,0);
const money=value=>new Intl.NumberFormat('en-US',{style:'currency',currency:'USD',maximumFractionDigits:0}).format(value);
const compact=value=>Math.abs(value)>=1000000?`$${(value/1000000).toFixed(1)}m`:Math.abs(value)>=1000?`$${(value/1000).toFixed(0)}k`:money(value);
const count=value=>Math.round(value).toLocaleString('en-US');
const percent=value=>`${Number(value).toFixed(1)}%`;
const escape=value=>String(value).replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
function bridge(data){
    const opening=sum(data.previous.revenue),closing=sum(data.current.revenue),net=closing-opening;
    const deltas=[opening,net*.8,net*.4,net*.2,-net*.25];deltas.push(closing-sum(deltas));deltas.push(closing);
    const labels=['Opening','New business','Expansion','Reactivation','Churn','Contraction','Closing'];let value=0;
    const totals=deltas.map((v,i)=>i===deltas.length-1?closing:(value+=v));
    const ranges=totals.map((v,i)=>i===0||i===totals.length-1?[0,v]:[Math.min(totals[i-1],v),Math.max(totals[i-1],v)]);
    return{labels,deltas,totals,ranges};
}
export function options(kind,data){
    const current=data.current,previous=data.previous,palette=chartPalette();
    const common=mergeOptions(baseChartOptions(document.documentElement.classList.contains('dark')),{
        chart:{type:'bar',animations:{enabled:!window.matchMedia('(prefers-reduced-motion: reduce)').matches,speed:500},toolbar:{show:true,tools:{download:true,selection:false,zoom:true,zoomin:true,zoomout:true,pan:true,reset:true}},zoom:{enabled:true,type:'x'}},
        colors:[palette[0],palette[3],palette[2]],stroke:{width:0},fill:{opacity:1},
        plotOptions:{bar:{horizontal:false,columnWidth:'52%',borderRadius:3}},
        grid:{strokeDashArray:3,padding:{left:2,right:12,top:8,bottom:0}},
        dataLabels:{enabled:false},
        legend:{show:true,position:'bottom',horizontalAlign:'left',fontSize:'10px',itemMargin:{horizontal:10,vertical:4}},
        xaxis:{type:'category',categories:data.labels,tickPlacement:'on',labels:{rotate:-35,rotateAlways:false,hideOverlappingLabels:true,trim:true,style:{fontSize:'10px'}}},
        yaxis:{min:0,tickAmount:4,labels:{formatter:count,style:{fontSize:'10px'},maxWidth:110}},
        tooltip:{shared:true,intersect:false,y:{formatter:count}},
        responsive:[{breakpoint:600,options:{legend:{fontSize:'9px',itemMargin:{horizontal:6,vertical:3}},xaxis:{labels:{style:{fontSize:'9px'}}}}}]
    });
    const horizontal={grid:{padding:{left:14,right:40,top:8,bottom:0}},chart:{toolbar:{show:false},zoom:{enabled:false}},plotOptions:{bar:{horizontal:true,barHeight:'48%'}},xaxis:{type:'category',labels:{rotate:0,formatter:count}},yaxis:{labels:{formatter:value=>String(value),style:{fontSize:'10px'},maxWidth:110}},legend:{show:false}};
    let config;
    switch(kind){
        case 'revenue':config={series:[{name:'Current revenue',data:current.revenue},{name:'Previous revenue',data:previous.revenue}],colors:[palette[0],palette[3]],yaxis:{labels:{formatter:compact}},tooltip:{y:{formatter:money}},fill:{type:['gradient','solid'],opacity:[1,.45],gradient:{type:'vertical',shadeIntensity:.12,opacityFrom:1,opacityTo:.55,stops:[0,100]}}};break;
        case 'region':config={series:[{name:'Actual revenue',data:current.regionalRevenue},{name:'Revenue target',data:current.regionalTargets}],xaxis:{categories:data.regions,labels:{formatter:label=>String(label).split(' ')}},colors:[palette[0],palette[2]],fill:{opacity:[1,.35]},yaxis:{labels:{formatter:compact}},tooltip:{y:{formatter:money}},plotOptions:{bar:{columnWidth:'60%'}}};break;
        case 'products':{
            const order=current.productRevenue.map((_,i)=>i).sort((a,b)=>current.productRevenue[b]-current.productRevenue[a]);
            config=mergeOptions(horizontal,{series:[{name:'Product revenue',data:order.map(i=>current.productRevenue[i])}],colors:palette,plotOptions:{bar:{distributed:true,barHeight:'46%'}},xaxis:{categories:order.map(i=>data.products[i]),labels:{formatter:compact}},tooltip:{shared:false,intersect:true,y:{formatter:money}}});break;
        }
        case 'workforce':config={chart:{stacked:true,stackType:'100',toolbar:{show:false},zoom:{enabled:false}},series:['On-site','Remote','Leave'].map((name,i)=>({name,data:current.workforce[i]})),colors:[palette[0],palette[3],palette[4]],xaxis:{categories:data.teams},yaxis:{min:0,max:100,tickAmount:4,labels:{formatter:v=>`${Math.round(v)}%`}},plotOptions:{bar:{columnWidth:'43%',borderRadius:0}},tooltip:{shared:false,intersect:true,custom:({seriesIndex,dataPointIndex})=>{const total=current.workforce.reduce((n,row)=>n+row[dataPointIndex],0),value=current.workforce[seriesIndex][dataPointIndex];return `<div class="bar-tooltip"><span>${escape(data.teams[dataPointIndex])}</span><br/><b>${['On-site','Remote','Leave'][seriesIndex]}: ${value} employees</b><br/>${percent(value/total*100)} of ${total} employees</div>`;}}};break;
        case 'delivery':config=mergeOptions(horizontal,{series:[{name:'Completed tasks',data:data.teams.map((x,i)=>({x,y:current.completed[i],goals:data.showTargets?[{name:'Planned tasks',value:current.planned[i],strokeColor:palette[4],strokeWidth:4,strokeHeight:12}]:[]}))}],colors:[palette[2]],xaxis:{categories:data.teams,max:Math.ceil(Math.max(...current.planned)*1.15)},tooltip:{shared:false,intersect:true,custom:({dataPointIndex:i})=>`<div class="bar-tooltip"><span>${escape(data.teams[i])}</span><br/><b>${current.completed[i]} of ${current.planned[i]} tasks</b><br/>${percent(current.completed[i]/current.planned[i]*100)} completed</div>`}});break;
        case 'pipeline':config=mergeOptions(horizontal,{series:[{name:'Stage volume',data:current.pipeline}],colors:palette,plotOptions:{bar:{distributed:true,barHeight:'60%',borderRadius:2}},xaxis:{categories:data.stages},tooltip:{shared:false,intersect:true,y:{formatter:(v,{dataPointIndex:i})=>`${count(v)} · ${percent(v/current.pipeline[0]*100)} of leads${i>0?` · ${percent(v/current.pipeline[i-1]*100)} from prior stage`:''}`}}});break;
        case 'sentiment':config=mergeOptions(horizontal,{chart:{stacked:true},series:[{name:'Negative feedback',data:current.negative.map((v,i)=>-v/(v+current.positive[i])*100)},{name:'Positive feedback',data:current.positive.map((v,i)=>v/(v+current.negative[i])*100)}],colors:[palette[1],palette[0]],plotOptions:{bar:{borderRadius:0,barHeight:'50%'}},xaxis:{categories:data.surveyLabels,min:-100,max:100,tickAmount:4,labels:{formatter:v=>`${Math.abs(Math.round(v))}%`}},yaxis:{min:-100,max:100,tickAmount:4},legend:{show:true},tooltip:{shared:false,intersect:true,custom:({seriesIndex,dataPointIndex:i})=>{const negative=seriesIndex===0,value=(negative?current.negative:current.positive)[i],total=current.positive[i]+current.negative[i];return `<div class="bar-tooltip"><span>${escape(data.surveyLabels[i])}</span><br/><b>${negative?'Negative':'Positive'}: ${value} of ${total} responses</b><br/>${percent(value/total*100)}</div>`;}}});break;
        case 'ranges':config=mergeOptions(horizontal,{chart:{type:'rangeBar'},series:[{name:'Estimated effort',data:data.releases.map((x,i)=>({x,y:current.ranges[i]}))}],colors:palette,plotOptions:{bar:{distributed:true,barHeight:'35%',borderRadius:3}},xaxis:{type:'numeric',categories:[],labels:{formatter:v=>`${Math.round(v)} pts`}},tooltip:{shared:false,intersect:true,custom:({dataPointIndex:i})=>`<div class="bar-tooltip"><span>${escape(data.releases[i])}</span><br/><b>${current.ranges[i][0]}–${current.ranges[i][1]} delivery points</b><br/>Uncertainty span: ${current.ranges[i][1]-current.ranges[i][0]} points</div>`}});break;
        case 'support':{
            const order=current.support.map((_,i)=>i).sort((a,b)=>data.descending?current.support[b]-current.support[a]:current.support[a]-current.support[b]);
            config=mergeOptions(horizontal,{series:[{name:'Open tickets',data:order.map(i=>current.support[i])}],colors:order.map(i=>palette[i]),plotOptions:{bar:{distributed:true,barHeight:'46%'}},xaxis:{categories:order.map(i=>data.queues[i])},tooltip:{shared:false,intersect:true,y:{formatter:v=>`${count(v)} open tickets`}}});break;
        }
        case 'waterfall':{
            const b=bridge(data);
            config={chart:{type:'rangeBar',toolbar:{show:false},zoom:{enabled:false}},series:[{name:'Revenue movement',data:b.labels.map((x,i)=>({x,y:b.ranges[i],fillColor:i===0||i===b.labels.length-1?palette[3]:b.deltas[i]>=0?palette[0]:palette[1]}))}],xaxis:{categories:b.labels},legend:{show:false},plotOptions:{bar:{horizontal:false,columnWidth:'45%',borderRadius:2}},yaxis:{labels:{formatter:compact}},tooltip:{shared:false,intersect:true,custom:({dataPointIndex:i})=>`<div class="bar-tooltip"><span>${b.labels[i]}</span><br/><b>${i===0||i===b.labels.length-1?'Revenue':'Contribution'}: ${money(b.deltas[i])}</b><br/>Cumulative revenue: ${money(b.totals[i])}</div>`}};break;
        }
        default:throw new Error(`Unsupported bar chart kind: ${kind}`);
    }
    return mergeOptions(common,config);
}
export function exportCsv(data,period,category){
    const c=data.current,p=data.previous,rows=[['Period','Category','Chart','Bucket','Metric','Current','Previous','Unit']];
    const add=(group,chart,bucket,metric,current,previous,unit)=>{if(category==='All Charts'||category===group)rows.push([period,group,chart,bucket,metric,current,previous,unit]);};
    data.labels.forEach((label,i)=>{add('Revenue','revenue',label,'Recognized revenue',c.revenue[i],p.revenue[i],'USD');add('Sales','region',label,'Revenue target',c.targets[i],p.targets[i],'USD');});
    data.regions.forEach((label,i)=>{add('Sales','region',label,'Actual revenue',c.regionalRevenue[i],p.regionalRevenue[i],'USD');add('Sales','region',label,'Planned revenue',c.regionalTargets[i],p.regionalTargets[i],'USD');});
    data.products.forEach((label,i)=>add('Revenue','products',label,'Product revenue',c.productRevenue[i],p.productRevenue[i],'USD'));
    data.teams.forEach((label,i)=>{['On-site','Remote','Leave'].forEach((name,j)=>{add('Teams','workforce',label,name,c.workforce[j][i],p.workforce[j][i],'employees');add('Teams','workforce',label,`${name} share`,c.workforce[j][i]/sum(c.workforce.map(row=>row[i]))*100,p.workforce[j][i]/sum(p.workforce.map(row=>row[i]))*100,'%');});add('Teams','delivery',label,'Planned tasks',c.planned[i],p.planned[i],'tasks');add('Teams','delivery',label,'Completed tasks',c.completed[i],p.completed[i],'tasks');add('Teams','delivery',label,'Completion rate',c.completed[i]/c.planned[i]*100,p.completed[i]/p.planned[i]*100,'%');});
    data.stages.forEach((label,i)=>add('Sales','pipeline',label,'Stage volume',c.pipeline[i],p.pipeline[i],'people'));
    data.surveyLabels.forEach((label,i)=>{add('Customers','sentiment',label,'Positive feedback',c.positive[i],p.positive[i],'responses');add('Customers','sentiment',label,'Negative feedback',c.negative[i],p.negative[i],'responses');add('Customers','sentiment',label,'Positive share',c.positive[i]/(c.positive[i]+c.negative[i])*100,p.positive[i]/(p.positive[i]+p.negative[i])*100,'%');});
    data.releases.forEach((label,i)=>{add('Operations','ranges',label,'Low effort estimate',c.ranges[i][0],p.ranges[i][0],'delivery points');add('Operations','ranges',label,'High effort estimate',c.ranges[i][1],p.ranges[i][1],'delivery points');});
    data.queues.forEach((label,i)=>add('Operations','support',label,'Open tickets',c.support[i],p.support[i],'tickets'));
    const b=bridge(data);b.labels.forEach((label,i)=>{add('Revenue','waterfall',label,'Contribution / endpoint',b.deltas[i],'','USD');add('Revenue','waterfall',label,'Cumulative revenue',b.totals[i],'','USD');});
    add('Revenue','Summary','All','Total revenue',sum(c.revenue),sum(p.revenue),'USD');add('Sales','Summary','All','Target attainment',sum(c.revenue)/sum(c.targets)*100,sum(p.revenue)/sum(p.targets)*100,'%');add('Teams','Summary','All','Delivery completion',sum(c.completed)/sum(c.planned)*100,sum(p.completed)/sum(p.planned)*100,'%');add('Operations','Summary','All','Open support tickets',sum(c.support),sum(p.support),'tickets');
    const csv=rows.map(row=>row.map(value=>`"${String(value).replaceAll('"','""')}"`).join(',')).join('\r\n');
    const url=URL.createObjectURL(new Blob(['\ufeff'+csv],{type:'text/csv;charset=utf-8'})),link=document.createElement('a');link.href=url;link.download=`rynzo-bar-${period.replaceAll(' ','-').toLowerCase()}.csv`;document.body.append(link);link.click();link.remove();setTimeout(()=>URL.revokeObjectURL(url),1000);
}


