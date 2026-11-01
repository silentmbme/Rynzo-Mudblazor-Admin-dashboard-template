import { baseChartOptions, mergeOptions, chartPalette, token } from '../vendor/apexcharts/source-options.js';
const money=value=>new Intl.NumberFormat('en-US',{style:'currency',currency:'USD',maximumFractionDigits:0}).format(value);
const short=value=>Math.abs(value)>=1000000?`${(value/1000000).toFixed(1)}m`:Math.abs(value)>=1000?`${(value/1000).toFixed(1)}k`:Number(value).toFixed(0);
const escape=value=>String(value).replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
const format=(value,unit)=>value===null?'Not yet observed':unit==='USD'?money(value):unit==='seconds'?`${value.toFixed(2)}s`:unit.includes('%')?`${value.toFixed(2)}%`:unit.includes('/ 100')?`${value.toFixed(1)} / 100`:Math.round(value).toLocaleString('en-US');
function rgb(color){if(color.startsWith('#')){let hex=color.slice(1);if(hex.length===3)hex=hex.split('').map(c=>c+c).join('');return[0,2,4].map(i=>parseInt(hex.slice(i,i+2),16));}const match=color.match(/[\d.]+/g);return match&&match.length>=3?match.slice(0,3).map(Number):[14,121,116];}
function mix(a,b,weight){const x=rgb(a),y=rgb(b);return '#'+x.map((v,i)=>Math.round(v*weight+y[i]*(1-weight)).toString(16).padStart(2,'0')).join('');}
function textColor(color){const [r,g,b]=rgb(color).map(v=>v/255).map(v=>v<=.04045?v/12.92:((v+.055)/1.055)**2.4);return .2126*r+.7152*g+.0722*b>.179?'#111827':'#ffffff';}
function rangesFor(matrix,data,palette){
    const values=matrix.values.flat().filter(v=>v!==null),card=token('--card');
    const make=(bounds,colors,names)=>bounds.slice(0,-1).map((from,i)=>({from:i?from+1e-7:from,to:bounds[i+1],color:colors[i],foreColor:textColor(colors[i]),name:names[i]}));
    const green=palette[0],orange=palette[1],red=token('--destructive','#dc4655');
    if(matrix.kind==='delta'){
        const limit=Math.max(10,Math.ceil(Math.max(...values.map(Math.abs))/5)*5),colors=[red,mix(red,card,.42),token('--muted'),mix(green,card,.5),green];
        return {min:-limit,max:limit,ranges:make([-limit,-5,-.01,.01,5,limit],colors,[`Below −5%`,'Decline','Near zero','Growth','Above +5%'])};
    }
    if(matrix.kind==='capacity')return{min:0,max:100,ranges:make([0,49.999999,data.threshold-.000001,100],[mix(green,card,.3),green,orange],['Under 50%',`50–<${data.threshold}%`,`Alert ≥${data.threshold}%`])};
    if(matrix.kind==='latency')return{min:0,max:6,ranges:make([0,1,2,3.5,6],[mix(green,card,.3),green,orange,red],['≤1s','1–2s','2–3.5s','>3.5s'])};
    if(matrix.kind==='risk')return{min:0,max:100,ranges:make([0,24.999,49.999,74.999,100],[mix(green,card,.4),green,orange,red],['Low <25','Moderate <50','Elevated <75','High ≥75'])};
    if(matrix.direction==='higher')return{min:0,max:100,ranges:make([0,60,75,90,100],[mix(green,card,.16),mix(green,card,.4),mix(green,card,.7),green],['0–60','60–75','75–90','90–100'])};
    if(matrix.kind==='conversion')return{min:0,max:8,ranges:make([0,2,4,6,8],[mix(palette[2],card,.15),mix(palette[2],card,.4),mix(palette[2],card,.7),palette[2]],['0–2%','2–4%','4–6%','6–8%'])};
    const max=Math.max(1,...values),bounds=[0,max*.25,max*.5,max*.75,max],tone=matrix.kind==='sales'?palette[3]:green;
    return{min:0,max,ranges:make(bounds,[.18,.4,.67,1].map(weight=>mix(tone,card,weight)),bounds.slice(0,-1).map((v,i)=>`${matrix.unit==='USD'?'$':''}${short(v)}–${matrix.unit==='USD'?'$':''}${short(bounds[i+1])}`))};
}
export function options(kind,data){
    const matrix=data.matrices.find(m=>m.kind===kind);
    if(!matrix)throw new Error(`Unsupported heatmap kind: ${kind}`);
    const palette=chartPalette(),scale=rangesFor(matrix,data,palette);
    const describe=(r,c)=>{
        const value=matrix.values[r]?.[c];if(value===undefined)return 'Select a cell to pin its details here.';
        const suffix=value===null?'':matrix.unit==='events'||matrix.unit==='sessions'?` ${matrix.unit}`:matrix.kind==='delta'?' vs. previous':'';
        return `${matrix.rows[r]} · ${matrix.columns[c]}: ${format(value,matrix.unit)}${suffix}`;
    };
    const reset=chart=>{if(!chart?.el)return;const detail=chart.el.closest('.heat-card')?.querySelector('.heat-selection');if(detail)detail.textContent='Select a cell to pin its details here.';chart.el.querySelectorAll('.apexcharts-heatmap-rect').forEach(cell=>{const r=Number(cell.getAttribute('i')),c=Number(cell.getAttribute('j'));if(matrix.values[r]?.[c]===null){cell.setAttribute('fill',token('--card'));cell.setAttribute('color',token('--card'));cell.setAttribute('aria-label',`${matrix.rows[r]}, ${matrix.columns[c]}: not yet observed`);}});};
    return mergeOptions(baseChartOptions(document.documentElement.classList.contains('dark')),{
        chart:{type:'heatmap',animations:{enabled:!window.matchMedia('(prefers-reduced-motion: reduce)').matches,speed:450},toolbar:{show:false},events:{mounted:chart=>reset(chart),updated:chart=>reset(chart),dataPointSelection:(_,chart,{seriesIndex,dataPointIndex})=>{const detail=chart.el.closest('.heat-card')?.querySelector('.heat-selection');if(detail)detail.textContent=describe(seriesIndex,dataPointIndex);}}},
        series:matrix.rows.map((name,r)=>({name,data:matrix.columns.map((x,c)=>({x,y:matrix.values[r][c]}))})),
        colors:[palette[0]],stroke:{width:2,colors:[token('--card')]},
        plotOptions:{heatmap:{radius:3,enableShades:false,distributed:false,shadeIntensity:0,useFillColorAsStroke:false,colorScale:scale}},
        dataLabels:{enabled:data.showValues,formatter:(value,{seriesIndex:r,dataPointIndex:c})=>matrix.values[r]?.[c]===null||value===null||value===undefined?'':matrix.unit==='USD'?`$${short(value)}`:matrix.unit==='seconds'?value.toFixed(1):matrix.unit.includes('%')?`${value.toFixed(0)}%`:short(value),style:{fontSize:'8px',fontWeight:500},background:{enabled:false}},
        xaxis:{type:'category',categories:matrix.columns,tickPlacement:'between',labels:{rotate:-35,rotateAlways:false,hideOverlappingLabels:true,trim:true,style:{fontSize:'9px'}}},
        yaxis:{reversed:true,labels:{minWidth:0,maxWidth:105,style:{fontSize:'9px'}}},
        grid:{show:false,padding:{left:10,right:25,top:6,bottom:0}},
        legend:{show:true,position:'bottom',horizontalAlign:'center',fontSize:'9px',itemMargin:{horizontal:7,vertical:4},onItemClick:{toggleDataSeries:false},onItemHover:{highlightDataSeries:false}},
        tooltip:{shared:false,intersect:true,custom:({seriesIndex:r,dataPointIndex:c})=>{
            const value=matrix.values[r]?.[c];if(value===undefined)return '';
            let detail='';const previous=matrix.previous[r][c];
            if(value!==null&&previous!==null)detail=`<br/>Previous: ${format(previous,matrix.unit)}`;
            if(kind==='conversion')detail+=`<br/>${matrix.numerators[r][c].toLocaleString('en-US')} converted / ${matrix.denominators[r][c].toLocaleString('en-US')} eligible leads`;
            if(kind==='retention'&&value!==null)detail+=`<br/>${Math.round(value/100*data.cohortSizes[r]).toLocaleString('en-US')} retained / ${data.cohortSizes[r].toLocaleString('en-US')} original members`;
            if(kind==='delta'){const sales=data.matrices.find(m=>m.kind==='sales');detail+=`<br/>Current: ${money(sales.values[r][c])}<br/>Previous: ${money(sales.previous[r][c])}`;}
            return `<div class="heat-tooltip"><span>${escape(matrix.rows[r])} · ${escape(matrix.columns[c])}</span><br/><b>${format(value,matrix.unit)}</b>${detail}${matrix.direction==='lower'?'<br/>Lower is preferable.':''}</div>`;
        }},
        responsive:[{breakpoint:1200,options:{dataLabels:{enabled:false}}},{breakpoint:600,options:{xaxis:{labels:{style:{fontSize:'8px'}}},legend:{fontSize:'8px',itemMargin:{horizontal:5,vertical:3}}}}]
    });
}
export function exportCsv(data,period,category){
    const rows=[['Period','Category','Chart','Row','Column','Current value','Previous value','Unit','Numerator','Denominator','Observed']];
    for(const m of data.matrices){if(category!=='All Charts'&&category!==m.category)continue;m.rows.forEach((row,r)=>m.columns.forEach((column,c)=>rows.push([period,m.category,m.kind,row,column,m.values[r][c]??'',m.previous[r][c]??'',m.unit,m.kind==='retention'&&m.values[r][c]!==null?Math.round(m.values[r][c]/100*data.cohortSizes[r]):m.numerators?.[r]?.[c]??'',m.kind==='retention'?data.cohortSizes[r]:m.denominators?.[r]?.[c]??'',m.values[r][c]!==null])));}
    const csv=rows.map(row=>row.map(value=>`"${String(value).replaceAll('"','""')}"`).join(',')).join('\r\n');
    const url=URL.createObjectURL(new Blob(['\ufeff'+csv],{type:'text/csv;charset=utf-8'})),link=document.createElement('a');link.href=url;link.download=`rynzo-heatmap-${period.replaceAll(' ','-').toLowerCase()}.csv`;document.body.append(link);link.click();link.remove();setTimeout(()=>URL.revokeObjectURL(url),1000);
}
