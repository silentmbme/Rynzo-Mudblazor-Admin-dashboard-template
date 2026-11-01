const sessions=new Map();
export function connect(id,receiver){const handler=e=>{if(e.target.matches('.conversation-draft textarea')&&e.key==='Enter'&&!e.shiftKey){e.preventDefault();receiver.invokeMethodAsync('SendFromKeyboard');}};document.addEventListener('keydown',handler);sessions.set(id,{handler,urls:[]});}
export function choose(id){document.getElementById(id)?.click();}
export function attachment(id){const file=document.getElementById(id)?.files?.[0];if(!file)return null;const image=file.type.startsWith('image/'),url=image?URL.createObjectURL(file):null;if(url)sessions.get(id)?.urls.push(url);return{name:file.name,type:image?'image':'file',url};}
export function disconnect(id){const session=sessions.get(id);if(!session)return;document.removeEventListener('keydown',session.handler);session.urls.forEach(url=>URL.revokeObjectURL(url));sessions.delete(id);}
