import { baseChartOptions, mergeOptions, chartPalette, token } from '../vendor/apexcharts/source-options.js';
const sum=values=>values.reduce((a,b)=>a+b,0);
const ratios=(a,b)=>a.map((v,i)=>v/b[i]*100);
const average=values=>sum(values)/values.length;
const rolling=(values,window)=>values.map((_,i)=>average(values.slice(Math.max(0,i-window+1),i+1)));
const money=value=>new Intl.NumberFormat('en-US',{style:'currency',currency:'USD',maximumFractionDigits:0}).format(value);
const compactMoney=value=>Math.abs(value)>=1000000?`$${(value/1000000).toFixed(1)}m`:Math.abs(value)>=1000?`$${(value/1000).toFixed(0)}k`:money(value);
const count=value=>Math.round(value).toLocaleString('en-US');
const compactCount=value=>Math.abs(value)>=1000000?`${(value/1000000).toFixed(1)}m`:Math.abs(value)>=1000?`${(value/1000).toFixed(0)}k`:count(value);
const percent=value=>`${Number(value).toFixed(1)}%`;
const channels=['Organic','Paid','Referral','Direct'];
const measures=f=>({activation:ratios(f.activated,f.eligible),retention:ratios(f.retained,f.cohort),resolution:ratios(f.resolved,f.due),net:f.receipts.map((v,i)=>v-f.payments[i])});
export function options(kind,data){
    const c=data.current,p=data.previous,a=measures(c),b=measures(p),palette=chartPalette();
    const base=mergeOptions(baseChartOptions(document.documentElement.classList.contains('dark')),{
        chart:{type:'line',animations:{enabled:!window.matchMedia('(prefers-reduced-motion: reduce)').matches,speed:500},toolbar:{show:true,tools:{download:true,selection:false,zoom:true,zoomin:true,zoomout:true,pan:true,reset:true}},zoom:{enabled:true,type:'x'}},
        colors:[palette[0],palette[3],palette[2],palette[4]],
        stroke:{width:2.5,curve:'smooth'},fill:{type:'solid',opacity:1},markers:{size:0,hover:{sizeOffset:4}},dataLabels:{enabled:false},
        grid:{strokeDashArray:3,padding:{left:6,right:30,top:10,bottom:0}},
        xaxis:{categories:data.labels,tickPlacement:'on',labels:{rotate:-35,rotateAlways:false,hideOverlappingLabels:true,trim:true,style:{fontSize:'10px'}}},
        yaxis:{min:0,tickAmount:4,labels:{formatter:compactCount,style:{fontSize:'10px'}}},
        legend:{show:true,position:'bottom',horizontalAlign:'left',fontSize:'10px',itemMargin:{horizontal:10,vertical:4}},
        tooltip:{shared:true,intersect:false,y:{formatter:count}},
        responsive:[{breakpoint:600,options:{legend:{fontSize:'9px',itemMargin:{horizontal:6,vertical:3}},xaxis:{labels:{style:{fontSize:'9px'}}}}}]
    });
    const percentage={min:0,max:100,tickAmount:4,labels:{formatter:v=>`${Math.round(v)}%`}};
    const reference=(value,text,color)=>({yaxis:[{y:value,borderColor:color,strokeDashArray:5,label:{text,style:{background:token('--card'),color:token('--foreground'),fontSize:'9px'}}}]});
    let config;
    switch(kind){
        case 'velocity':config={series:[{name:'Actual revenue',data:c.revenue},{name:'Previous revenue',data:p.revenue},{name:'Revenue plan',data:c.plan}],colors:[palette[0],palette[3],palette[2]],stroke:{width:[3,2,2],dashArray:[0,5,2]},yaxis:{labels:{formatter:compactMoney}},tooltip:{y:{formatter:money}}};break;
        case 'acquisition':config={series:channels.map((name,i)=>({name,data:c.channels[i]})),colors:[palette[0],palette[3],palette[2],palette[4]],stroke:{curve:'straight',width:2,dashArray:[0,4,0,2]},markers:{size:[2,0,2,0]},tooltip:{y:{formatter:v=>`${count(v)} registrations`}}};break;
        case 'resolution':config={series:[{name:'Resolved within service window',data:a.resolution}],colors:[palette[2]],stroke:{curve:'stepline',width:3},legend:{show:false},yaxis:percentage,tooltip:{y:{formatter:(v,{dataPointIndex:i})=>`${percent(v)} · ${c.resolved[i]} of ${c.due[i]} tickets due`}},annotations:reference(90,'90% reference',palette[4])};break;
        case 'activation':config={series:[{name:'Activation rate',data:a.activation},{name:'Activation goal',data:c.eligible.map(()=>65)}],colors:[palette[0],palette[4]],stroke:{curve:'straight',width:[3,2],dashArray:[0,6]},markers:{size:[3,0]},yaxis:percentage,tooltip:{y:{formatter:(v,{seriesIndex,dataPointIndex:i})=>seriesIndex===0?`${percent(v)} · ${c.activated[i]} of ${c.eligible[i]} registrations`:percent(v)}}};break;
        case 'retention':config={series:[{name:'Current retention',data:a.retention},{name:'Previous retention',data:b.retention}],colors:[palette[2],palette[3]],stroke:{width:[3,2],dashArray:[0,5]},yaxis:percentage,tooltip:{y:{formatter:(v,{seriesIndex,dataPointIndex:i})=>{const f=seriesIndex===0?c:p;return `${percent(v)} · ${f.retained[i]} of ${f.cohort[i]} accounts`;}}},annotations:reference(90,'90% retention reference',palette[4])};break;
        case 'latency':config={series:[{name:'Median (P50)',data:c.p50},{name:'Tail latency (P95)',data:c.p95}],colors:[palette[0],palette[1]],stroke:{curve:'straight',width:[2,3]},markers:{size:[3,4],strokeWidth:2,strokeColors:token('--card')},yaxis:{min:0,max:Math.ceil(Math.max(...c.p95,data.sla)*1.2*10)/10,labels:{formatter:v=>`${v.toFixed(1)}s`}},tooltip:{y:{formatter:v=>`${v.toFixed(2)} seconds`}},annotations:reference(data.sla,`${data.sla.toFixed(1)}s P95 threshold`,palette[1])};break;
        case 'forecast':{
            const n=c.revenue.length,points=values=>data.forecastLabels.map((x,i)=>({x,y:i>=n-1?values[i-n+1]:null}));
            config={series:[{name:'Historical revenue',data:data.forecastLabels.map((x,i)=>({x,y:i<n?c.revenue[i]:null}))},{name:'Projected revenue',data:points(data.projection)},{name:'Lower scenario',data:points(data.lower)},{name:'Upper scenario',data:points(data.upper)}],xaxis:{categories:data.forecastLabels},colors:[palette[0],palette[2],palette[3],palette[3]],stroke:{curve:'straight',width:[3,3,1.5,1.5],dashArray:[0,6,2,2]},yaxis:{labels:{formatter:compactMoney}},tooltip:{y:{formatter:money}},annotations:{xaxis:[{x:data.labels.at(-1),borderColor:token('--muted-foreground'),strokeDashArray:4,label:{text:'Forecast begins',style:{background:token('--card'),color:token('--foreground'),fontSize:'9px'}}}]}};break;
        }
        case 'rolling':config={series:[{name:'Revenue',data:c.revenue},{name:`${data.window}-bucket moving average`,data:rolling(c.revenue,data.window)}],colors:[palette[3],palette[2]],stroke:{curve:'straight',width:[1.5,3.5]},markers:{size:[3,0]},yaxis:{labels:{formatter:compactMoney}},tooltip:{y:{formatter:money}}};break;
        case 'cashflow':config={series:[{name:'Receipts',data:c.receipts},{name:'Payments',data:c.payments},{name:'Net cash',data:a.net}],colors:[palette[0],palette[1],palette[2]],stroke:{curve:'straight',width:[2,2,3],dashArray:[0,4,0]},yaxis:{min:Math.min(0,...a.net)*1.2,labels:{formatter:compactMoney}},tooltip:{y:{formatter:money}},annotations:{yaxis:[{y:0,borderColor:token('--muted-foreground'),strokeDashArray:0}]}};break;
        case 'momentum':{
            const names=['Revenue','Activation','Retention'],raw=[c.revenue,a.activation,a.retention],visible=names.map((_,i)=>i).filter(i=>data.focus==='All metrics'||data.focus===names[i]);
            config={series:visible.map(i=>({name:names[i],data:raw[i].map(v=>v/raw[i][0]*100)})),colors:visible.map(i=>[palette[0],palette[3],palette[2]][i]),stroke:{curve:'straight',width:2.5},yaxis:{labels:{formatter:v=>`${Math.round(v)} idx`}},tooltip:{y:{formatter:(v,{seriesIndex,dataPointIndex:i})=>{const source=visible[seriesIndex];return `${v.toFixed(1)} index · ${source===0?money(raw[source][i]):percent(raw[source][i])} original`;}}},annotations:{yaxis:[{y:100,borderColor:token('--muted-foreground'),strokeDashArray:5}]}};break;
        }
        default:throw new Error(`Unsupported line chart kind: ${kind}`);
    }
    return mergeOptions(base,config);
}
export function exportCsv(data,period,category){
    const c=data.current,p=data.previous,a=measures(c),b=measures(p),cr=rolling(c.revenue,data.window),pr=rolling(p.revenue,data.window),rows=[['Period','Category','Chart','Bucket','Metric','Current','Previous','Unit']];
    const add=(group,chart,label,metric,current,previous,unit)=>{if(category==='All Charts'||category===group)rows.push([period,group,chart,label,metric,current,previous,unit]);};
    data.labels.forEach((label,i)=>{
        add('Revenue','velocity',label,'Recognized revenue',c.revenue[i],p.revenue[i],'USD');add('Revenue','velocity',label,'Revenue plan',c.plan[i],p.plan[i],'USD');
        channels.forEach((name,j)=>add('Marketing','acquisition',label,name,c.channels[j][i],p.channels[j][i],'registrations'));
        add('Operations','resolution',label,'Tickets due',c.due[i],p.due[i],'tickets');add('Operations','resolution',label,'Resolved tickets',c.resolved[i],p.resolved[i],'tickets');add('Operations','resolution',label,'Resolution rate',a.resolution[i],b.resolution[i],'%');
        add('Customers','activation',label,'Eligible registrations',c.eligible[i],p.eligible[i],'registrations');add('Customers','activation',label,'Activated registrations',c.activated[i],p.activated[i],'registrations');add('Customers','activation',label,'Activation rate',a.activation[i],b.activation[i],'%');add('Customers','activation',label,'Activation goal',65,65,'%');
        add('Customers','retention',label,'Eligible cohort',c.cohort[i],p.cohort[i],'accounts');add('Customers','retention',label,'Retained accounts',c.retained[i],p.retained[i],'accounts');add('Customers','retention',label,'Retention rate',a.retention[i],b.retention[i],'%');
        add('Operations','latency',label,'Median latency P50',c.p50[i],p.p50[i],'seconds');add('Operations','latency',label,'Tail latency P95',c.p95[i],p.p95[i],'seconds');add('Operations','latency',label,'P95 threshold',data.sla,'','seconds');
        add('Revenue','rolling',label,`${data.window}-bucket moving average`,cr[i],pr[i],'USD');
        add('Finance','cashflow',label,'Receipts',c.receipts[i],p.receipts[i],'USD');add('Finance','cashflow',label,'Payments',c.payments[i],p.payments[i],'USD');add('Finance','cashflow',label,'Net cash',a.net[i],b.net[i],'USD');
        ['Revenue','Activation','Retention'].forEach((name,j)=>{if(data.focus==='All metrics'||data.focus===name){const current=[c.revenue,a.activation,a.retention][j],previous=[p.revenue,b.activation,b.retention][j];add('Revenue','momentum',label,`${name} index`,current[i]/current[0]*100,previous[i]/previous[0]*100,'index: first bucket = 100');}});
    });
    data.projection.forEach((v,i)=>{const label=data.forecastLabels[c.revenue.length-1+i];add('Revenue','forecast',label,'Projected revenue',v,'','USD');add('Revenue','forecast',label,'Lower scenario',data.lower[i],'','USD');add('Revenue','forecast',label,'Upper scenario',data.upper[i],'','USD');});
    add('Revenue','Summary','All','Total revenue',sum(c.revenue),sum(p.revenue),'USD');add('Customers','Summary','All','Weighted activation rate',sum(c.activated)/sum(c.eligible)*100,sum(p.activated)/sum(p.eligible)*100,'%');add('Customers','Summary','All','Weighted retention rate',sum(c.retained)/sum(c.cohort)*100,sum(p.retained)/sum(p.cohort)*100,'%');add('Operations','Summary','Latest','Latest P95 latency',c.p95.at(-1),p.p95.at(-1),'seconds');
    const csv=rows.map(row=>row.map(value=>`"${String(value).replaceAll('"','""')}"`).join(',')).join('\r\n');
    const url=URL.createObjectURL(new Blob(['\ufeff'+csv],{type:'text/csv;charset=utf-8'})),link=document.createElement('a');link.href=url;link.download=`rynzo-line-${period.replaceAll(' ','-').toLowerCase()}.csv`;document.body.append(link);link.click();link.remove();setTimeout(()=>URL.revokeObjectURL(url),1000);
}
