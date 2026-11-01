export async function copyPromotion(){
    if(!navigator.clipboard?.writeText)return false;
    try { await navigator.clipboard.writeText('AURORA20');return true; } catch {return false;}
}
