import { baseChartOptions, mergeOptions, chartPalette, token } from '../vendor/apexcharts/source-options.js';

const score = value => Number(value).toFixed(1);
const escape = value => String(value).replace(/[&<>"']/g, c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
const mean = values => values.reduce((a,b)=>a+b,0)/values.length;
const categoryFor = kind => kind==='marketing'?'Marketing':kind==='risk'?'Risk':kind==='project'?'Projects':kind==='experience'?'Customers':kind==='departments'||kind==='skills'||kind.startsWith('team')?'Teams':'Business';
const wrapLabel = value => String(value).split(' ');

export function options(kind, data) {
    const profile = data.profiles.find(p=>p.kind===kind);
    if (!profile) throw new Error(`Unsupported radar chart kind: ${kind}`);
    const palette=chartPalette(), border=token('--border'), muted=token('--muted');
    const compact=/^team[0-3]$/.test(kind);
    const visible=profile.series.map((_,i)=>i).filter(i=>kind!=='product'||data.visibleProducts.includes(i));
    const series=visible.map(i=>({name:profile.series[i].name,data:profile.series[i].values}));
    const base=mergeOptions(baseChartOptions(document.documentElement.classList.contains('dark')),{
        chart:{type:'radar',animations:{enabled:!window.matchMedia('(prefers-reduced-motion: reduce)').matches,speed:550},toolbar:{show:false},dropShadow:{enabled:false}},
        series,labels:profile.dimensions,colors:palette,
        stroke:{width:2,curve:'straight',dashArray:0},
        fill:{type:'solid',opacity:.11},
        markers:{size:3,strokeWidth:1.5,strokeColors:token('--card'),hover:{sizeOffset:2}},
        dataLabels:{enabled:false},
        legend:{show:true,position:'bottom',horizontalAlign:'center',fontSize:'10px',itemMargin:{horizontal:9,vertical:5},onItemClick:{toggleDataSeries:true}},
        grid:{padding:{top:8,right:26,bottom:4,left:26}},
        plotOptions:{radar:{polygons:{strokeColors:border,connectorColors:border,strokeWidth:1,fill:{colors:['transparent',muted,'transparent',muted,'transparent']}}}},
        xaxis:{categories:profile.dimensions,labels:{show:true,formatter:wrapLabel,style:{fontSize:compact?'8px':'10px',fontWeight:400,colors:profile.dimensions.map(()=>token('--muted-foreground'))}}},
        yaxis:{show:!compact,min:0,max:100,tickAmount:4,forceNiceScale:false,labels:{formatter:value=>String(Math.round(value)),style:{fontSize:'8px',colors:[token('--muted-foreground')]}}},
        tooltip:{shared:false,intersect:true,custom:({seriesIndex,dataPointIndex})=>{
            const originalIndex=visible[seriesIndex], item=profile.series[originalIndex], value=item?.values[dataPointIndex];
            if(value===undefined)return '';
            const ceiling=profile.ceilings[dataPointIndex], unit=profile.units[dataPointIndex];
            const raw=value*ceiling/100;
            const referenceIndex=kind==='executive'?2:kind==='growth'?2:profile.series.length===2?1:-1;
            let detail=ceiling===100&&unit==='index / 100'?'':`<br/>Original: ${score(raw)} ${escape(unit)}<br/>Reference ceiling: ${ceiling.toLocaleString('en-US')}`;
            if(referenceIndex>=0&&originalIndex!==referenceIndex){const target=profile.series[referenceIndex].values[dataPointIndex];detail+=`<br/>${kind==='risk'?'Acceptable threshold':'Reference'}: ${score(target)} / 100<br/>Gap: ${value-target>0?'+':''}${score(value-target)} points`;}
            return `<div class="radar-tooltip"><span>${escape(profile.dimensions[dataPointIndex])}</span><br/><b>${escape(item.name)}: ${score(value)} / 100</b>${detail}${kind==='risk'?'<br/>Higher values mean greater risk.':''}</div>`;
        }},
        responsive:[{breakpoint:600,options:{legend:{itemMargin:{horizontal:5,vertical:4},fontSize:'9px'},xaxis:{labels:{style:{fontSize:compact?'8px':'9px'}}}}}]
    });
    let style={};
    switch(kind){
        case 'executive': style={colors:[palette[0],palette[3],palette[2]],stroke:{dashArray:[0,5,2]},fill:{type:['gradient','solid','solid'],opacity:[.2,.06,0],gradient:{type:'diagonal1',shadeIntensity:.1,opacityFrom:.24,opacityTo:.04,stops:[0,100]}},markers:{size:[4,2,0]}};break;
        case 'departments': style={colors:palette.slice(0,5),fill:{opacity:.045},markers:{size:3,shape:['circle','square','diamond','triangle','plus']},plotOptions:{radar:{polygons:{fill:{colors:['transparent']}}}}};break;
        case 'skills': style={colors:[palette[2],palette[4]],stroke:{dashArray:[0,6]},fill:{opacity:[.18,0]},markers:{size:[4,3]},plotOptions:{radar:{offsetX:-10,polygons:{fill:{colors:[muted,'transparent']}}}}};break;
        case 'product': style={colors:visible.map(i=>palette[i]),legend:{show:false},fill:{opacity:.09},markers:{size:4,shape:visible.map(i=>['circle','square','diamond'][i])}};break;
        case 'experience': style={colors:[palette[0],palette[3]],fill:{opacity:[.24,0]},stroke:{dashArray:[0,7]},markers:{size:[4,0]}};break;
        case 'marketing': style={colors:palette.slice(0,6),fill:{opacity:.025},stroke:{width:1.8},markers:{size:2,shape:['circle','square','diamond','triangle','plus','cross']},plotOptions:{radar:{polygons:{fill:{colors:['transparent']}}}}};break;
        case 'risk': style={colors:[token('--destructive',palette[1]),token('--muted-foreground')],fill:{opacity:[.14,0]},stroke:{dashArray:[0,5]},markers:{size:[5,2],discrete:profile.series[0].values.map((v,i)=>({seriesIndex:0,dataPointIndex:i,fillColor:v>=60?token('--destructive',palette[1]):v>=35?palette[4]:palette[0],strokeColor:token('--card'),size:5,shape:'circle'}))}};break;
        case 'project': style={colors:[palette[3],palette[4]],fill:{opacity:[.13,.025]},stroke:{dashArray:[0,3]},markers:{size:[4,2],discrete:profile.series[0].values.flatMap((v,i)=>v<profile.series[1].values[i]?[{seriesIndex:0,dataPointIndex:i,fillColor:palette[1],strokeColor:token('--card'),size:5}]:[])}};break;
        case 'growth': style={colors:[palette[0],palette[2],palette[4]],fill:{opacity:[.16,.045,0]},stroke:{dashArray:[0,6,2]},markers:{size:[4,3,0]},plotOptions:{radar:{polygons:{fill:{colors:['transparent']}}}}};break;
        default:
            if(!compact)throw new Error(`Missing radar configuration: ${kind}`);
            style={colors:[palette[Number(kind.at(-1))],token('--muted-foreground')],fill:{opacity:[.16,0]},stroke:{width:1.8,dashArray:[0,4]},markers:{size:[2,0]},legend:{fontSize:'8px',itemMargin:{horizontal:5,vertical:3}},grid:{padding:{top:2,right:24,left:24,bottom:0}}};
    }
    return mergeOptions(base,style);
}

export function exportCsv(data,period,category) {
    const rows=[['Period','Category','Profile','Series','Dimension','Normalized score / 100','Original value','Original unit','Reference ceiling']];
    for(const p of data.profiles){
        const group=categoryFor(p.kind);
        if(category!=='All Charts'&&category!==group)continue;
        p.series.forEach((series,index)=>{
            if(p.kind==='product'&&!data.visibleProducts.includes(index))return;
            series.values.forEach((value,i)=>rows.push([period,group,p.kind,series.name,p.dimensions[i],value,Number((value*p.ceilings[i]/100).toFixed(4)),p.units[i],p.ceilings[i]]));
        });
    }
    if(category==='All Charts'||category==='Business'){
        const executive=data.profiles.find(p=>p.kind==='executive'),target=executive.series[2].values;
        executive.series.slice(0,2).forEach(series=>{
            rows.push([period,'Business','Summary',series.name,'Overall performance',mean(series.values),'','index / 100',100]);
            rows.push([period,'Business','Summary',series.name,'Below-target dimensions',series.values.filter((v,i)=>v<target[i]).length,'','count',6]);
            rows.push([period,'Business','Summary',series.name,'Average benchmark achievement',mean(series.values.map((v,i)=>100*v/target[i])),'','% of target','']);
            const top=series.values.indexOf(Math.max(...series.values));rows.push([period,'Business','Summary',series.name,`Top dimension: ${executive.dimensions[top]}`,series.values[top],'','index / 100',100]);
        });
    }
    const csv=rows.map(row=>row.map(value=>`"${String(value).replaceAll('"','""')}"`).join(',')).join('\r\n');
    const url=URL.createObjectURL(new Blob(['\ufeff'+csv],{type:'text/csv;charset=utf-8'})),link=document.createElement('a');link.href=url;link.download=`rynzo-radar-${period.replaceAll(' ','-').toLowerCase()}.csv`;document.body.append(link);link.click();link.remove();setTimeout(()=>URL.revokeObjectURL(url),1000);
}


