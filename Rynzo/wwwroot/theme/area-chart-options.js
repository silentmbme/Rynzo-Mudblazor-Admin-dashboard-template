import { baseChartOptions, mergeOptions, chartPalette, token } from '../vendor/apexcharts/source-options.js';
const sum=values=>values.reduce((a,b)=>a+b,0);
const sessions=f=>f.revenue.map((_,i)=>sum(f.traffic.map(row=>row[i])));
const cumulative=values=>{let total=0;return values.map(v=>total+=v);};
const money=value=>new Intl.NumberFormat('en-US',{style:'currency',currency:'USD',maximumFractionDigits:0}).format(value);
const compactMoney=value=>Math.abs(value)>=1000000?`$${(value/1000000).toFixed(1)}m`:Math.abs(value)>=1000?`$${(value/1000).toFixed(0)}k`:money(value);
const count=value=>Math.round(value).toLocaleString('en-US');
const compactCount=value=>Math.abs(value)>=1000000?`${(value/1000000).toFixed(1)}m`:Math.abs(value)>=1000?`${(value/1000).toFixed(0)}k`:count(value);
const percent=value=>`${Number(value).toFixed(1)}%`;
const channels=['Organic','Paid','Referral','Direct'];
export function options(kind,data){
    const c=data.current,p=data.previous,palette=chartPalette(),traffic=sessions(c);
    const base=mergeOptions(baseChartOptions(document.documentElement.classList.contains('dark')),{
        chart:{type:'area',animations:{enabled:!window.matchMedia('(prefers-reduced-motion: reduce)').matches,speed:500},toolbar:{show:true,tools:{download:true,selection:false,zoom:true,zoomin:true,zoomout:true,pan:true,reset:true}},zoom:{enabled:true,type:'x'}},
        colors:[palette[0],palette[3],palette[2],palette[4]],
        xaxis:{categories:data.labels,tickPlacement:'on',labels:{rotate:-35,rotateAlways:false,hideOverlappingLabels:true,trim:true,style:{fontSize:'10px'}}},
        yaxis:{min:0,tickAmount:4,labels:{formatter:compactCount,style:{fontSize:'10px'}}},
        grid:{strokeDashArray:3,padding:{left:6,right:28,top:10,bottom:0}},
        stroke:{width:2.5,curve:'smooth'},fill:{type:'gradient',gradient:{type:'vertical',shadeIntensity:.1,opacityFrom:.32,opacityTo:.025,stops:[0,100]}},
        dataLabels:{enabled:false},markers:{size:0,hover:{sizeOffset:4}},
        legend:{show:true,position:'bottom',horizontalAlign:'left',fontSize:'10px',itemMargin:{horizontal:10,vertical:4}},
        tooltip:{shared:true,intersect:false,y:{formatter:count}},
        responsive:[{breakpoint:600,options:{legend:{fontSize:'9px',itemMargin:{horizontal:6,vertical:3}},xaxis:{labels:{style:{fontSize:'9px'}}}}}]
    });
    let config;
    switch(kind){
        case 'coverage':config={series:[{name:'Actual revenue',type:'area',data:c.revenue},{name:'Previous revenue',type:'area',data:p.revenue},{name:'Revenue plan',type:'line',data:c.target}],colors:[palette[0],palette[3],palette[2]],stroke:{width:[3,2,2],dashArray:[0,0,6]},fill:{type:['gradient','solid','solid'],opacity:[.2,.06,1],gradient:{opacityFrom:.3,opacityTo:.015}},yaxis:{labels:{formatter:compactMoney}},tooltip:{y:{formatter:money}}};break;
        case 'traffic':config={chart:{stacked:true},series:channels.map((name,i)=>({name,data:c.traffic[i]})),colors:[palette[0],palette[3],palette[2],palette[4]],stroke:{width:1.5,curve:'straight'},fill:{type:'solid',opacity:.72},tooltip:{y:{formatter:v=>`${count(v)} sessions`}}};break;
        case 'confidence':{
            const n=c.revenue.length;
            config={chart:{type:'line'},series:[{name:'Forecast range',type:'rangeArea',data:data.forecastLabels.map((x,i)=>({x,y:i>=n-1?[data.lower[i-n+1],data.upper[i-n+1]]:[null,null]}))},{name:'Historical revenue',type:'line',data:data.forecastLabels.map((x,i)=>({x,y:i<n?c.revenue[i]:null}))},{name:'Revenue projection',type:'line',data:data.forecastLabels.map((x,i)=>({x,y:i>=n-1?data.projection[i-n+1]:null}))}],xaxis:{categories:data.forecastLabels},colors:[palette[3],palette[0],palette[2]],fill:{type:'solid',opacity:[.19,1,1]},stroke:{curve:'straight',width:[0,3,3],dashArray:[0,0,6]},yaxis:{labels:{formatter:compactMoney}},tooltip:{y:{formatter:money}},annotations:{xaxis:[{x:data.labels.at(-1),borderColor:token('--muted-foreground'),strokeDashArray:4,label:{text:'Forecast begins',style:{background:token('--card'),color:token('--foreground'),fontSize:'9px'}}}]}};break;
        }
        case 'engagement':config={series:[{name:'Engaged sessions',data:c.engagement},{name:'Previous engaged sessions',data:p.engagement}],colors:[palette[2],palette[3]],stroke:{width:[3,2],dashArray:[0,5]},tooltip:{y:{formatter:v=>`${count(v)} engaged sessions`}}};break;
        case 'allocation':config={chart:{stacked:true,stackType:'100'},series:channels.map((name,i)=>({name,data:c.traffic[i]})),colors:[palette[0],palette[3],palette[2],palette[4]],stroke:{width:1,curve:'straight'},fill:{type:'solid',opacity:.72},yaxis:{min:0,max:100,tickAmount:4,labels:{formatter:v=>`${Math.round(v)}%`}},tooltip:{shared:false,intersect:false,y:{formatter:(v,{seriesIndex,dataPointIndex:i})=>`${count(c.traffic[seriesIndex][i])} sessions · ${percent(c.traffic[seriesIndex][i]/traffic[i]*100)} of all traffic`}}};break;
        case 'capacity':config={series:[{name:'Capacity utilization',data:c.capacity}],colors:[palette[4]],stroke:{curve:'stepline',width:3},legend:{show:false},yaxis:{min:0,max:100,tickAmount:4,labels:{formatter:v=>`${Math.round(v)}%`}},tooltip:{y:{formatter:percent}},annotations:{yaxis:[{y:data.threshold,borderColor:palette[1],strokeDashArray:5,label:{text:`${data.threshold}% threshold`,style:{background:token('--card'),color:token('--foreground'),fontSize:'9px'}}}]}};break;
        case 'cashflow':{
            const net=c.receipts.map((v,i)=>v-c.payments[i]);
            config={series:[{name:'Net cash movement',data:net}],colors:[palette[2]],stroke:{curve:'straight',width:2.5},legend:{show:false},yaxis:{min:Math.min(0,...net)*1.2,max:Math.max(...net)*1.2,labels:{formatter:compactMoney}},tooltip:{y:{formatter:(v,{dataPointIndex:i})=>`${money(v)} net · ${money(c.receipts[i])} receipts · ${money(c.payments[i])} payments`}},annotations:{yaxis:[{y:0,borderColor:token('--muted-foreground'),strokeDashArray:0}]}};break;
        }
        case 'customers':config={series:[{name:'Cumulative net additions',data:cumulative(c.acquired.map((v,i)=>v-c.churned[i]))},{name:'Previous net additions',data:cumulative(p.acquired.map((v,i)=>v-p.churned[i]))}],colors:[palette[0],palette[4]],stroke:{curve:'straight',width:[3,2],dashArray:[0,5]},tooltip:{y:{formatter:v=>`${count(v)} net additions in window`}}};break;
        case 'support':config={series:[{name:'Tickets opened',data:c.opened},{name:'Tickets resolved',data:c.resolved}],colors:[palette[1],palette[0]],stroke:{curve:'stepline',width:2},fill:{type:'solid',opacity:[.13,.13]},tooltip:{y:{formatter:v=>`${count(v)} tickets`}}};break;
        case 'momentum':{
            const raw=[c.revenue,traffic,c.engagement],names=['Revenue','Traffic','Engagement'],visible=names.map((_,i)=>i).filter(i=>data.focus==='All metrics'||data.focus===names[i]);
            config={series:visible.map(i=>({name:names[i],data:raw[i].map(v=>v/raw[i][0]*100)})),colors:visible.map(i=>[palette[0],palette[3],palette[2]][i]),stroke:{curve:'straight',width:2.5},tooltip:{y:{formatter:(v,{seriesIndex,dataPointIndex})=>{const i=visible[seriesIndex];return `${v.toFixed(1)} index · ${i===0?money(raw[i][dataPointIndex]):count(raw[i][dataPointIndex])+' sessions'} original`;}}},yaxis:{labels:{formatter:v=>Math.round(v)+' idx'}},annotations:{yaxis:[{y:100,borderColor:token('--muted-foreground'),strokeDashArray:5}]}};break;
        }
        default:throw new Error(`Unsupported area chart kind: ${kind}`);
    }
    return mergeOptions(base,config);
}
export function exportCsv(data,period,category){
    const c=data.current,p=data.previous,cs=sessions(c),ps=sessions(p),cn=cumulative(c.acquired.map((v,i)=>v-c.churned[i])),pn=cumulative(p.acquired.map((v,i)=>v-p.churned[i]));
    const rows=[['Period','Category','Chart','Bucket','Metric','Current','Previous','Unit']];
    const add=(group,chart,bucket,name,current,previous,unit)=>{if(category==='All Charts'||category===group)rows.push([period,group,chart,bucket,name,current,previous,unit]);};
    data.labels.forEach((label,i)=>{
        add('Revenue','coverage',label,'Recognized revenue',c.revenue[i],p.revenue[i],'USD');add('Revenue','coverage',label,'Revenue plan',c.target[i],p.target[i],'USD');
        channels.forEach((name,j)=>{add('Marketing','traffic',label,name,c.traffic[j][i],p.traffic[j][i],'sessions');add('Marketing','allocation',label,`${name} share`,c.traffic[j][i]/cs[i]*100,p.traffic[j][i]/ps[i]*100,'%');});
        add('Customers','engagement',label,'Engaged sessions',c.engagement[i],p.engagement[i],'sessions');add('Operations','capacity',label,'Capacity utilization',c.capacity[i],p.capacity[i],'%');add('Operations','capacity',label,'Operating threshold',data.threshold,'','%');
        add('Finance','cashflow',label,'Receipts',c.receipts[i],p.receipts[i],'USD');add('Finance','cashflow',label,'Payments',c.payments[i],p.payments[i],'USD');add('Finance','cashflow',label,'Net cash movement',c.receipts[i]-c.payments[i],p.receipts[i]-p.payments[i],'USD');
        add('Customers','customers',label,'Acquired customers',c.acquired[i],p.acquired[i],'customers');add('Customers','customers',label,'Churned customers',c.churned[i],p.churned[i],'customers');add('Customers','customers',label,'Cumulative net additions',cn[i],pn[i],'customers');
        add('Operations','support',label,'Opened tickets',c.opened[i],p.opened[i],'tickets');add('Operations','support',label,'Resolved tickets',c.resolved[i],p.resolved[i],'tickets');
        ['Revenue','Traffic','Engagement'].forEach((name,j)=>{if(data.focus==='All metrics'||data.focus===name){const cr=[c.revenue,cs,c.engagement][j],pr=[p.revenue,ps,p.engagement][j];add('Revenue','momentum',label,`${name} index`,cr[i]/cr[0]*100,pr[i]/pr[0]*100,'index: first bucket = 100');}});
    });
    data.projection.forEach((value,i)=>{const label=data.forecastLabels[c.revenue.length-1+i];add('Revenue','confidence',label,'Revenue projection',value,'','USD');add('Revenue','confidence',label,'Lower forecast bound',data.lower[i],'','USD');add('Revenue','confidence',label,'Upper forecast bound',data.upper[i],'','USD');});
    add('Revenue','Summary','All','Total revenue',sum(c.revenue),sum(p.revenue),'USD');add('Marketing','Summary','All','Total sessions',sum(cs),sum(ps),'sessions');add('Customers','Summary','All','Net additions',cn.at(-1),pn.at(-1),'customers');add('Operations','Summary','Latest','Latest capacity',c.capacity.at(-1),p.capacity.at(-1),'%');add('Operations','support','Opening','Opening backlog',c.openingBacklog,p.openingBacklog,'tickets');add('Operations','support','Closing','Closing backlog',c.openingBacklog+sum(c.opened)-sum(c.resolved),p.openingBacklog+sum(p.opened)-sum(p.resolved),'tickets');
    const csv=rows.map(row=>row.map(value=>`"${String(value).replaceAll('"','""')}"`).join(',')).join('\r\n');
    const url=URL.createObjectURL(new Blob(['\ufeff'+csv],{type:'text/csv;charset=utf-8'})),link=document.createElement('a');link.href=url;link.download=`rynzo-area-${period.replaceAll(' ','-').toLowerCase()}.csv`;document.body.append(link);link.click();link.remove();setTimeout(()=>URL.revokeObjectURL(url),1000);
}
