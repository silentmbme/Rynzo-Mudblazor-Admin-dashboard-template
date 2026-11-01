export function goBack() {
    let internalReferrer = false;
    try { internalReferrer = new URL(document.referrer).origin === location.origin; } catch { /* Direct visit. */ }
    // Blazor records an index for same-document navigation; external history is not a safe fallback.
    if (history.length > 1 && (history.state?._index > 0 || internalReferrer)) history.back();
    else location.assign(new URL('/', location.origin));
}
export function retry() { location.reload(); }
export async function dashboardReachable() {
    const controller = new AbortController();
    const timeout = setTimeout(() => controller.abort(), 5000);
    try {
        const response = await fetch(new URL('/', location.origin), { method: 'GET', cache: 'no-store', credentials: 'same-origin', signal: controller.signal, headers: { Accept: 'text/html' } });
        const reachable = response.ok && new URL(response.url).origin === location.origin && response.headers.get('content-type')?.includes('text/html');
        await response.body?.cancel();
        return Boolean(reachable);
    } catch { return false; }
    finally { clearTimeout(timeout); }
}
