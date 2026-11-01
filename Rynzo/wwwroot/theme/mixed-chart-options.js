import { baseChartOptions, mergeOptions, chartPalette, token } from '../vendor/apexcharts/source-options.js';
const sum = values => values.reduce((a,b)=>a+b,0);
const pct = value => `${Number(value).toFixed(1)}%`;
const money = value => new Intl.NumberFormat('en-US',{style:'currency',currency:'USD',maximumFractionDigits:0}).format(value);
const compact = value => Math.abs(value)>=1000?`$${(value/1000).toFixed(0)}k`:`$${Math.round(value)}`;
const count = value => Math.round(value).toLocaleString('en-US');
const channelNames=['Organic','Paid','Referral','Direct'];
const series=(name,type,data)=>({name,type,data});
const axis=(names,title,formatter,opposite=false,max)=>({seriesName:names,title:{text:title,style:{fontSize:'10px',fontWeight:500}},opposite,min:0,...(max===undefined?{}:{max}),forceNiceScale:max===undefined,tickAmount:4,labels:{formatter,style:{fontSize:'10px'}}});
const derived = d => ({
 growth:d.revenue.map((v,i)=>(v/(i?d.revenue[i-1]:d.priorBucket)-1)*100),
 achievement:d.revenue.map((v,i)=>v/d.salesTarget[i]*100),
 rate:d.conversions.map((v,i)=>v/d.visitors[i]*100),
 profit:d.revenue.map((v,i)=>v-d.operatingCosts[i]-d.otherCosts[i]),
 margin:d.revenue.map((v,i)=>(v-d.operatingCosts[i]-d.otherCosts[i])/v*100),
 roi:d.spend.map((v,i)=>(d.attributedRevenue[i]-v)/v*100),
 completion:d.completed.map((v,i)=>v/d.planned[i]*100)
});

export function options(kind,d) {
 const p=chartPalette(), a=derived(d), dark=document.documentElement.classList.contains('dark');
 const common=mergeOptions(baseChartOptions(dark),{
  chart:{type:'line',stacked:false,toolbar:{show:true,tools:{download:true,selection:false,zoom:true,zoomin:true,zoomout:true,pan:true,reset:true}},zoom:{enabled:true,type:'x'},animations:{enabled:true,speed:500}},
  colors:[p[0],p[3],p[2],p[1],p[4]],legend:{show:true,position:'bottom',horizontalAlign:'left',fontSize:'11px',itemMargin:{horizontal:10,vertical:4}},
  xaxis:{categories:d.labels,tickPlacement:'on',labels:{rotate:-35,rotateAlways:false,trim:true,hideOverlappingLabels:true,style:{fontSize:'10px'}}},
  grid:{strokeDashArray:3,padding:{top:10,right:8,left:4,bottom:0}},
  stroke:{width:[0,3],curve:'smooth'},markers:{size:0,hover:{sizeOffset:3}},fill:{opacity:1},dataLabels:{enabled:false},
  tooltip:{shared:true,intersect:false,y:{formatter:(v,{seriesIndex,w})=>{const name=w.config.series[seriesIndex]?.name||'';return /growth|achievement|rate|margin|ROI|SLA/i.test(name)?pct(v):/revenue|sales|cost|expense|profit|spend|contribution|opening|cumulative|projection|historical|range/i.test(name)?money(v):count(v);}}},
  responsive:[{breakpoint:600,options:{legend:{fontSize:'10px',itemMargin:{horizontal:5}}}}]
 });
 let config;
 switch(kind){
 case 'revenue': config={series:[series('Revenue','column',d.revenue),series('Revenue growth','line',a.growth)],yaxis:[axis('Revenue','Revenue · USD',compact),{...axis('Revenue growth','Growth · %',pct,true),min:Math.min(0,...a.growth)-2}],plotOptions:{bar:{columnWidth:'48%',borderRadius:4}},fill:{type:['gradient','solid'],gradient:{type:'vertical',shadeIntensity:.15,opacityFrom:1,opacityTo:.45,stops:[0,100]}},colors:[p[0],p[2]]};break;
 case 'sales':config={series:[series('Actual sales','column',d.revenue),series('Target sales','column',d.salesTarget),series('Achievement','line',a.achievement)],yaxis:[axis(['Actual sales','Target sales'],'Sales · USD',compact),axis('Achievement','Achievement · %',pct,true,Math.max(120,...a.achievement)+5)],stroke:{width:[0,0,3]},colors:[p[0],p[3],p[2]],plotOptions:{bar:{columnWidth:'60%',borderRadius:2}},annotations:{xaxis:d.labels.flatMap((label,i)=>a.achievement[i]>=100?[{x:label,borderColor:p[0],strokeDashArray:0,opacity:.12,label:{text:'✓',style:{color:p[0],background:token('--card'),fontSize:'9px'}}}]:[])}};break;
 case 'website':config={series:[series('Visitors','area',d.visitors),series('Conversions','column',d.conversions),series('Conversion rate','line',a.rate)],colors:[p[3],p[0],p[2]],stroke:{width:[2,0,3]},fill:{type:['gradient','solid','solid'],opacity:[.22,.9,1],gradient:{opacityFrom:.25,opacityTo:.02,stops:[0,100]}},yaxis:[axis(['Visitors','Conversions'],'Visitors / conversions',count),axis('Conversion rate','Conversion · %',pct,true,Math.ceil(Math.max(...a.rate)+1))],plotOptions:{bar:{columnWidth:'32%'}},dataLabels:{enabled:true,enabledOnSeries:[2],formatter:pct,offsetY:-6,style:{fontSize:'9px'},background:{enabled:true,borderRadius:3,padding:3,opacity:.85}}};break;
 case 'profitability':config={chart:{stacked:true,stackOnlyBar:true},series:[series('Operating expenses','column',d.operatingCosts),series('Other costs','column',d.otherCosts),series('Net profit','line',a.profit),series('Profit margin','line',a.margin)],colors:[p[3],p[2],p[0],p[4]],stroke:{width:[0,0,3,2],dashArray:[0,0,0,5]},yaxis:[axis(['Operating expenses','Other costs','Net profit'],'Costs / profit · USD',compact),axis('Profit margin','Margin · %',pct,true,50)],plotOptions:{bar:{columnWidth:'48%',borderRadius:2}}};break;
 case 'marketing':config={series:[series('Campaign spend','column',d.spend),series('Attributed revenue','line',d.attributedRevenue),series('ROI','scatter',a.roi)],xaxis:{categories:d.campaigns},colors:[p[3],p[0],p[2]],yaxis:[axis(['Campaign spend','Attributed revenue'],'Spend / revenue · USD',compact),axis('ROI','ROI · %',pct,true,Math.ceil(Math.max(...a.roi)/50)*50+50)],stroke:{width:[0,3,0]},markers:{size:[0,3,7],strokeWidth:2,strokeColors:token('--card')},plotOptions:{bar:{columnWidth:'35%',borderRadius:3}},tooltip:{shared:false,intersect:true}};break;
 case 'acquisition':config={chart:{stacked:true,stackOnlyBar:false},series:[...channelNames.map((name,i)=>series(name,'area',d.acquisition[i])),series('Total acquisitions','line',d.conversions)],colors:[p[0],p[3],p[2],p[4],token('--foreground')],stroke:{width:[1,1,1,1,3]},fill:{opacity:[.75,.65,.65,.6,1]},yaxis:axis([...channelNames,'Total acquisitions'],'New customers',count,false,Math.ceil(Math.max(...d.conversions)*1.15)),markers:{size:[0,0,0,0,3]}};break;
 case 'product':config={chart:{type:'bar',zoom:{enabled:false},toolbar:{show:false}},series:[series('Product revenue','bar',d.productRevenue)],xaxis:{categories:d.products,labels:{formatter:compact,style:{fontSize:'9px'}},title:{text:'Revenue · USD',style:{fontSize:'10px'}}},yaxis:{labels:{maxWidth:90,style:{fontSize:'10px'}}},plotOptions:{bar:{horizontal:true,barHeight:'45%',borderRadius:3,distributed:true}},colors:p,legend:{show:false},stroke:{width:0},tooltip:{shared:false,intersect:true,y:{formatter:money}},dataLabels:{enabled:false}};break;
 case 'product-satisfaction':config={chart:{type:'scatter',toolbar:{show:false},zoom:{enabled:false}},series:[series('Satisfaction','scatter',d.satisfaction.map((score,i)=>({x:score,y:d.products.length-i})))],xaxis:{type:'numeric',categories:[],min:0,max:100,tickAmount:4,title:{text:'Score / 100',style:{fontSize:'10px'}},labels:{formatter:v=>Number(v).toFixed(0)}},yaxis:{min:0,max:d.products.length+1,tickAmount:d.products.length+1,labels:{formatter:v=>d.products[d.products.length-Math.round(v)]||'',style:{fontSize:'9px'},maxWidth:90}},markers:{size:7,strokeWidth:2,strokeColors:token('--card')},colors:[p[2]],stroke:{width:0},legend:{show:false},tooltip:{shared:false,intersect:true,custom:({dataPointIndex})=>`<div class="mixed-product-tooltip">${d.products[dataPointIndex]}<br/><b>${d.satisfaction[dataPointIndex]} / 100</b></div>`}};break;
 case 'operations':config={series:[series('Completed tasks','column',d.completed),series('SLA target','line',d.sla),series('Completion rate','line',a.completion)],colors:[p[0],p[4],p[2]],stroke:{width:[0,2,3],curve:['straight','stepline','smooth'],dashArray:[0,5,0]},yaxis:[axis('Completed tasks','Completed tasks',count),axis(['SLA target','Completion rate'],'Completion · %',pct,true,100)],annotations:{yaxis:[{y:92,yAxisIndex:1,borderColor:p[4],strokeDashArray:5,label:{text:'92% service threshold',style:{background:token('--card'),color:token('--muted-foreground'),fontSize:'9px'}}}]},plotOptions:{bar:{columnWidth:'42%',borderRadius:3}}};break;
 case 'forecast':{
  const n=d.revenue.length;
  const history=d.forecastLabels.map((x,i)=>({x,y:i<n?d.revenue[i]:null}));
  const projected=d.forecastLabels.map((x,i)=>({x,y:i>=n-1?d.projection[i-n+1]:null}));
  const band=d.forecastLabels.map((x,i)=>({x,y:i>=n-1?[d.low[i-n+1],d.high[i-n+1]]:[null,null]}));
  config={series:[series('Forecast range','rangeArea',band),series('Historical revenue','line',history),series('Revenue projection','line',projected)],xaxis:{categories:d.forecastLabels},colors:[p[3],p[0],p[2]],fill:{opacity:[.18,1,1]},stroke:{width:[0,3,3],dashArray:[0,0,6],curve:'straight'},yaxis:axis(['Forecast range','Historical revenue','Revenue projection'],'Revenue · USD',compact),annotations:{xaxis:[{x:d.labels.at(-1),borderColor:token('--muted-foreground'),strokeDashArray:4,label:{text:'Forecast begins',style:{background:token('--card'),color:token('--foreground'),fontSize:'9px'}}}]},tooltip:{shared:true,intersect:false,y:{formatter:money}}};break;
 }
 case 'waterfall':{
  let cumulative=0;
  const totals=d.contributions.map((v,i)=>i===d.contributions.length-1?cumulative:(cumulative+=v));
  const ranges=d.growthLabels.map((x,i)=>({x,y:i===0||i===d.growthLabels.length-1?[0,totals[i]]:[Math.min(totals[i-1],totals[i]),Math.max(totals[i-1],totals[i])],fillColor:i===0||i===d.growthLabels.length-1?p[3]:d.contributions[i]>=0?p[0]:p[1]}));
  config={series:[series('Revenue contribution','rangeBar',ranges),series('Cumulative revenue','line',d.growthLabels.map((x,i)=>({x,y:totals[i]})))],xaxis:{categories:d.growthLabels},plotOptions:{bar:{horizontal:false,columnWidth:'48%',borderRadius:2}},colors:[p[0],p[2]],stroke:{width:[0,3],curve:'straight'},markers:{size:[0,4]},yaxis:axis(['Revenue contribution','Cumulative revenue'],'Revenue · USD',compact),tooltip:{shared:false,intersect:true,custom:({dataPointIndex:i})=>`<div class="mixed-product-tooltip"><b>${d.growthLabels[i]}</b><br/>${i===0||i===d.growthLabels.length-1?'Revenue':d.contributions[i]>=0?'Positive contribution':'Negative contribution'}: ${money(d.contributions[i])}<br/>Cumulative revenue: ${money(totals[i])}</div>`}};break;
 }
 default:throw new Error(`Unsupported mixed chart kind: ${kind}`);
 }
 return mergeOptions(common,config);
}

export function exportCsv(d,period,category) {
 const a=derived(d),rows=[['Period','Category','Bucket','Metric','Value','Unit']];
 const add=(group,bucket,name,value,unit)=>{if(category==='All Charts'||category===group)rows.push([period,group,bucket,name,value,unit]);};
 d.labels.forEach((label,i)=>{
  [['Revenue',d.revenue[i],'USD'],['Revenue growth',a.growth[i],'%'],['Sales target',d.salesTarget[i],'USD'],['Achievement',a.achievement[i],'%']].forEach(([name,value,unit])=>add('Revenue',label,name,value,unit));
  [['Visitors',d.visitors[i],'count'],['Conversions',d.conversions[i],'count'],['Conversion rate',a.rate[i],'%']].forEach(([name,value,unit])=>add('Customers',label,name,value,unit));
  channelNames.forEach((name,c)=>add('Customers',label,name,d.acquisition[c][i],'customers'));
  [['Operating expenses',d.operatingCosts[i],'USD'],['Other costs',d.otherCosts[i],'USD'],['Net profit',a.profit[i],'USD'],['Profit margin',a.margin[i],'%']].forEach(([name,value,unit])=>add('Finance',label,name,value,unit));
  add('Operations',label,'Completed tasks',d.completed[i],'tasks');add('Operations',label,'Planned tasks',d.planned[i],'tasks');add('Operations',label,'SLA target',d.sla[i],'%');add('Operations',label,'Completion rate',a.completion[i],'%');
 });
 d.campaigns.forEach((name,i)=>{add('Marketing',name,'Campaign spend',d.spend[i],'USD');add('Marketing',name,'Attributed revenue',d.attributedRevenue[i],'USD');add('Marketing',name,'ROI',a.roi[i],'%');});
 d.products.forEach((name,i)=>{add('Revenue',name,'Product revenue',d.productRevenue[i],'USD');add('Revenue',name,'Satisfaction',d.satisfaction[i],'score / 100');});
 d.projection.forEach((value,i)=>{const label=d.forecastLabels[d.revenue.length-1+i];add('Finance',label,'Projection',value,'USD');add('Finance',label,'Lower forecast range',d.low[i],'USD');add('Finance',label,'Upper forecast range',d.high[i],'USD');});
 let cumulative=0;d.contributions.forEach((value,i)=>{if(i<d.contributions.length-1)cumulative+=value;add('Revenue',d.growthLabels[i],'Contribution',value,'USD');add('Revenue',d.growthLabels[i],'Cumulative revenue',cumulative,'USD');});
 add('Revenue','Summary','Total revenue',sum(d.revenue),'USD');add('Customers','Summary','Conversion rate',sum(d.conversions)/sum(d.visitors)*100,'%');add('Marketing','Summary','Spend-weighted average ROI',(sum(d.attributedRevenue)-sum(d.spend))/sum(d.spend)*100,'%');add('Finance','Summary','Profit margin',sum(a.profit)/sum(d.revenue)*100,'%');
 const csv=rows.map(row=>row.map(value=>`"${String(value).replaceAll('"','""')}"`).join(',')).join('\r\n');
 const url=URL.createObjectURL(new Blob(['\ufeff'+csv],{type:'text/csv;charset=utf-8'})),link=document.createElement('a');link.href=url;link.download=`rynzo-mixed-${period.replaceAll(' ','-').toLowerCase()}.csv`;document.body.append(link);link.click();link.remove();setTimeout(()=>URL.revokeObjectURL(url),1000);
}

