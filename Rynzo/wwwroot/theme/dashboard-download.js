export function csv(filename,contents){
    const url=URL.createObjectURL(new Blob([contents],{type:'text/csv;charset=utf-8;'}));
    const anchor=document.createElement('a');anchor.href=url;anchor.download=filename;anchor.click();
    setTimeout(()=>URL.revokeObjectURL(url),1000);
}
