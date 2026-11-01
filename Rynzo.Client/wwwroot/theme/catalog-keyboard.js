const handlers=new WeakMap();
export function connect(element){const handler=e=>{if(e.target.matches('article[data-sku]')&&(e.key===' '||e.key==='Enter'))e.preventDefault();};element.addEventListener('keydown',handler);handlers.set(element,handler);}
export function disconnect(element){const handler=handlers.get(element);if(handler)element.removeEventListener('keydown',handler);handlers.delete(element);}
