const key='rynzo-calendar-events-v1';
export function load(){const now=new Date();let saved=null;try{saved=localStorage.getItem(key);}catch{}return{today:`${now.getFullYear()}-${String(now.getMonth()+1).padStart(2,'0')}-${String(now.getDate()).padStart(2,'0')}`,mobile:matchMedia('(max-width: 639px)').matches,saved};}
export function save(json){try{localStorage.setItem(key,json);return true;}catch{return false;}}
export function download(filename,contents){const url=URL.createObjectURL(new Blob([contents],{type:'text/calendar;charset=utf-8'}));const link=document.createElement('a');link.href=url;link.download=filename;link.click();setTimeout(()=>URL.revokeObjectURL(url),1000);}
export function scrollTimeline(element){element.scrollTop=7*48;}
