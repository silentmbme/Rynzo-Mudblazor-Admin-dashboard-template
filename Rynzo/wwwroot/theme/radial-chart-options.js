import { baseChartOptions, mergeOptions, token, chartPalette } from '../vendor/apexcharts/source-options.js';

const labels = {
 executive: ['Revenue target','Sales achievement','Customer retention','Operational efficiency'],
 departments: ['Marketing','Sales','Finance','Operations','Customer Support'],
 funnel: ['Leads generated','Qualified leads','Opportunities','Converted customers'],
 time: ['Productive work','Meetings','Development','Administration','Remaining time']
};
const percent = value => `${Number(value).toFixed(1).replace(/\.0$/, '')}%`;
export function options(kind, data) {
 const palette = chartPalette(), muted = token('--muted'), foreground = token('--foreground');
 const base = mergeOptions(baseChartOptions(document.documentElement.classList.contains('dark')), {
  chart: {type:'radialBar', animations:{enabled:true,speed:650}}, colors:palette,
  legend:{show:false}, grid:{padding:{top:4,bottom:4,left:0,right:0}},
  stroke:{lineCap:'round'}, tooltip:{enabled:true, y:{formatter:percent}},
  plotOptions:{radialBar:{hollow:{size:'64%',background:'transparent'},track:{background:muted,strokeWidth:'100%',margin:7},dataLabels:{name:{show:true,offsetY:24,fontSize:'12px'},value:{show:true,offsetY:-10,fontSize:'30px',fontWeight:650,color:foreground,formatter:percent}}}}
 });
 let config;
 const rings = (values, names, total, totalLabel, hollow='42%') => {
  const kept = values.map((v,i)=>i).filter(i=>!data.hidden.includes(kind+i));
  // Keep the chart valid when every legend entry is hidden, without inventing a data value.
  return {series:kept.length?kept.map(i=>values[i]):[0],labels:kept.length?kept.map(i=>names[i]):['All metrics hidden'],colors:kept.length?kept.map(i=>palette[i]):[muted],plotOptions:{radialBar:{hollow:{size:hollow},dataLabels:{total:{show:true,label:totalLabel,formatter:()=>total}}}}};
 };
 if(kind==='executive') config=rings(data.executive,labels.executive,percent(data.overall),'Overall score');
 else if(kind==='revenue') config={series:[data.executive[0]],labels:['Target achieved'],colors:[palette[0]],fill:{type:'gradient',gradient:{shade:'light',type:'horizontal',shadeIntensity:.2,gradientToColors:[palette[3]],stops:[0,100]}}};
 else if(kind==='satisfaction') {
  // Four zone bands are implemented as supported radial tracks; the inner arc is the actual score.
  const zoneColors=[token('--destructive','#dc4655'),palette[1],palette[4],palette[0]];
  config={series:[data.satisfaction],labels:['Satisfaction'],colors:[palette[0]],plotOptions:{radialBar:{startAngle:-90,endAngle:90,hollow:{size:'66%'},track:{startAngle:-90,endAngle:90},dataLabels:{name:{offsetY:26},value:{offsetY:-8}}}},chart:{events:{mounted(chart){decorateSatisfaction(chart,zoneColors,foreground);},updated(chart){decorateSatisfaction(chart,zoneColors,foreground);}}}};
 }
 else if(kind==='departments') config=mergeOptions(rings(data.departments,labels.departments,percent(data.departments.reduce((a,b)=>a+b,0)/5),'Team average','24%'),{plotOptions:{radialBar:{startAngle:-135,endAngle:135,track:{startAngle:-135,endAngle:135,margin:9}}}});
 else if(kind==='goals') config={chart:{type:'donut'},series:Array(20).fill(1),labels:Array.from({length:20},(_,i)=>`Milestone ${i+1} · ${i<data.completed?'Completed':'Pending'}`),colors:Array.from({length:20},(_,i)=>i<data.completed?palette[2]:muted),stroke:{width:4,colors:[token('--card')]},dataLabels:{enabled:false},plotOptions:{pie:{expandOnClick:true,donut:{size:'77%',labels:{show:true,name:{show:true},value:{show:false},total:{show:true,showAlways:true,label:'Goals complete',formatter:()=>percent(data.completed*5)}}}}},tooltip:{y:{formatter:()=> '1 of 20 milestones'}}};
 else if(/^system[0-3]$/.test(kind)||/^finance[0-3]$/.test(kind)) {
  const financial=kind.startsWith('finance'),index=Number(kind.at(-1)),value=(financial?data.financial:data.systemValues)[index];
  config={series:[value],labels:[financial?'Score / 100':'Utilization'],colors:[financial?(value>=80?palette[0]:palette[4]):value>=85?token('--destructive','#dc4655'):value>=65?palette[4]:palette[0]],plotOptions:{radialBar:{hollow:{size:'65%'},track:{margin:0},dataLabels:{name:{fontSize:'9px',offsetY:18},value:{fontSize:'22px',offsetY:-12,formatter:v=>financial?Number(v).toFixed(0):percent(v)}}}}};
 }
 else if(kind==='funnel') {
  const values=data.funnel.map(v=>v/data.funnel[0]*100);
  config=mergeOptions(rings(values,labels.funnel,percent(values[3]),'Lead conversion','35%'),{stroke:{lineCap:'butt'},plotOptions:{radialBar:{track:{margin:10}}},tooltip:{y:{formatter:(v,{seriesIndex})=>{const active=data.funnel.map((_,i)=>i).filter(i=>!data.hidden.includes(kind+i));const i=active[seriesIndex];return i===undefined?'All stages hidden':`${data.funnel[i].toLocaleString()} · ${percent(values[i])} of leads`;}}}});
 }
 else if(kind==='time') {
  const active=data.hours.map((_,i)=>i).filter(i=>!data.hidden.includes(kind+i));
  config={chart:{type:'donut'},series:active.length?active.map(i=>data.hours[i]):[0],labels:active.length?active.map(i=>labels.time[i]):['All activities hidden'],colors:active.length?active.map(i=>palette[i]):[muted],stroke:{width:5,colors:[token('--card')]},dataLabels:{enabled:false},plotOptions:{pie:{startAngle:-60,endAngle:300,donut:{size:'68%',labels:{show:true,value:{formatter:v=>`${v}h`},total:{show:true,showAlways:true,label:'Weekly capacity',formatter:()=> '40h'}}}}},tooltip:{y:{formatter:v=>`${v} hours · ${percent(v/40*100)}`}}};
 }
 else throw new Error(`Unsupported radial chart kind: ${kind}`);
 return mergeOptions(base,config);
}

function decorateSatisfaction(chart, colors, foreground) {
 const svg=chart.el.querySelector('svg'),group=chart.el.querySelector('.apexcharts-radialbar');
 if(!svg||!group)return;
 svg.querySelector('.satisfaction-reference')?.remove();
 const track=group.querySelector('.apexcharts-radialbar-track path');
 if(!track)return;
 const box=track.getBBox(),cx=box.x+box.width/2,cy=box.y+box.height,r=box.width/2+10;
 const ns='http://www.w3.org/2000/svg', overlay=document.createElementNS(ns,'g');overlay.setAttribute('class','satisfaction-reference');overlay.setAttribute('pointer-events','none');
 const point=(angle,radius)=>[cx+Math.cos(angle)*radius,cy+Math.sin(angle)*radius];
 [0,40,60,80].forEach((start,i)=>{const a=point(Math.PI+start/100*Math.PI,r),b=point(Math.PI+[40,60,80,100][i]/100*Math.PI,r),path=document.createElementNS(ns,'path');path.setAttribute('d',`M ${a} A ${r} ${r} 0 0 1 ${b}`);path.setAttribute('fill','none');path.setAttribute('stroke',colors[i]);path.setAttribute('stroke-width','4');overlay.append(path);});
 const a=point(Math.PI*1.9,r-9),b=point(Math.PI*1.9,r+7),line=document.createElementNS(ns,'line');['x1','y1','x2','y2'].forEach((key,i)=>line.setAttribute(key,[...a,...b][i]));line.setAttribute('stroke',foreground);line.setAttribute('stroke-width','2');const title=document.createElementNS(ns,'title');title.textContent='Satisfaction target: 90%';line.append(title);overlay.append(line);group.append(overlay);
}

export function exportCsv(data, period, category) {
 const rows=[['Period','Category','Metric','Value','Unit']];
 const add=(group,name,value,unit)=>{if(category==='All Charts'||category===group)rows.push([period,group,name,value,unit]);};
 labels.executive.forEach((name,i)=>add('Performance',name,data.executive[i],'%'));
 add('Performance','Overall performance',data.overall,'%');
 add('Revenue','Revenue target',data.target,'USD');add('Revenue','Revenue achieved',data.achieved,'USD');add('Revenue','Revenue achievement',data.executive[0],'%');
 add('Customers','Customer satisfaction',data.satisfaction,'%');
 labels.departments.forEach((name,i)=>add('Performance',name,data.departments[i],'%'));
 add('Operations','Completed milestones',data.completed,'of 20');
 ['CPU','Memory','Storage','Network'].forEach((name,i)=>add('Operations',name,data.systemValues[i],'%'));
 labels.funnel.forEach((name,i)=>{add('Revenue',name,data.funnel[i],'count');add('Revenue',`${name} / leads`,data.funnel[i]/data.funnel[0]*100,'%');});
 ['Profitability','Liquidity','Revenue stability','Cost efficiency'].forEach((name,i)=>add('Finance',name,data.financial[i],'score / 100'));
 add('Finance','Overall financial health',data.financial.reduce((a,b)=>a+b,0)/4,'score / 100');
 labels.time.forEach((name,i)=>add('Operations',name,data.hours[i],'hours / week'));
 const csv=rows.map(row=>row.map(value=>`"${String(value).replaceAll('"','""')}"`).join(',')).join('\r\n');
 const url=URL.createObjectURL(new Blob(['\ufeff'+csv],{type:'text/csv;charset=utf-8'})),link=document.createElement('a');link.href=url;link.download=`rynzo-radial-${period.replaceAll(' ','-').toLowerCase()}.csv`;document.body.append(link);link.click();link.remove();setTimeout(()=>URL.revokeObjectURL(url),1000);
}

