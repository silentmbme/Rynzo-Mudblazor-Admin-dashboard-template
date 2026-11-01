// Existing Rynzo appearance/layout preferences only; never authentication state.
(() => {
    const skins = ['multicolor', 'sapphire', 'emerald', 'violet', 'rose', 'orange', 'cyan', 'graphite'];
    const media = window.matchMedia('(prefers-color-scheme: dark)');
    let receiver, current;
    const defaults = { mode: 'light', skin: 'multicolor', sidebarSize: 'default', sidebarColor: 'light', sidebarCollapsed: false, direction: 'ltr', density: 'comfortable', header: 'fixed', layoutWidth: 'fluid' };
    function normalize(saved) {
        const state = { ...defaults, ...saved };
        const choices = { mode: ['light', 'dark', 'system'], skin: skins, sidebarSize: ['default', 'compact'], sidebarColor: ['light', 'dark'], direction: ['ltr', 'rtl'], density: ['comfortable', 'compact'], header: ['fixed', 'static'], layoutWidth: ['fluid', 'boxed'] };
        for (const [key, values] of Object.entries(choices)) if (!values.includes(state[key])) state[key] = defaults[key];
        state.sidebarCollapsed = state.sidebarCollapsed === true;
        state.isDark = state.mode === 'dark' || state.mode === 'system' && media.matches;
        return state;
    }
    function stored() {
        try { const saved = JSON.parse(localStorage.getItem('rynzo-admin-settings') || '{}'); return saved && typeof saved === 'object' && !Array.isArray(saved) ? saved : {}; } catch { return {}; }
    }
    function read() {
        const saved = stored();
        if (saved.themeVersion !== 3) saved.skin = defaults.skin;
        try { const color = localStorage.getItem('rynzo-sidebar-color'); if (['light', 'dark'].includes(color)) saved.sidebarColor = color; } catch {}
        return normalize(saved);
    }
    function apply(state) {
        const root = document.documentElement;
        root.classList.add('rynzo-theme'); root.classList.toggle('dark', state.isDark);
        root.dataset.theme = state.isDark ? 'dark' : 'light'; root.dataset.skin = state.skin;
        root.dir = state.direction; root.dataset.sidebarColor = state.sidebarColor;
        root.dataset.density = state.density; root.dataset.layoutWidth = state.layoutWidth;
        root.dataset.sidebarCollapsed = String(state.sidebarCollapsed || state.sidebarSize === 'compact');
        return current = state;
    }
    function notify(event) {
        if (event?.type === 'storage' && !['rynzo-admin-settings', 'rynzo-sidebar-color', null].includes(event.key)) return;
        const state = apply(event?.type === 'storage' ? read() : normalize(current || read()));
        if (receiver) receiver.invokeMethodAsync('AppearanceChanged', state).catch(() => {});
    }
    function setOptions(patch) {
        const allowed = Object.fromEntries(Object.entries(patch).filter(([key]) => Object.hasOwn(defaults, key)));
        const state = normalize({ ...(current || read()), ...allowed });
        try {
            const { isDark, ...persisted } = state;
            localStorage.setItem('rynzo-admin-settings', JSON.stringify({ ...stored(), ...persisted, themeVersion: 3 }));
        } catch {}
        // React persists sidebar color independently: one failed key must not prevent the other write.
        if (Object.hasOwn(allowed, 'sidebarColor')) {
            try { localStorage.setItem('rynzo-sidebar-color', state.sidebarColor); } catch {}
        }
        return apply(state);
    }
    function reset() {
        const state = normalize(defaults);
        try {
            const { isDark, ...persisted } = state;
            localStorage.setItem('rynzo-admin-settings', JSON.stringify({ ...persisted, themeVersion: 3 }));
            localStorage.removeItem('rynzo-sidebar-color');
        } catch {}
        return apply(state);
    }
    window.rynzoTheme = {
        connect(dotnet) { receiver = dotnet; media.addEventListener('change', notify); window.addEventListener('storage', notify); return apply(read()); },
        setMode(mode) { if (!['light', 'dark', 'system'].includes(mode)) throw Error('Invalid mode'); return setOptions({ mode }); },
        setOptions,
        reset,
        disconnect() { receiver = undefined; media.removeEventListener('change', notify); window.removeEventListener('storage', notify); }
    };
    apply(read());
})();
