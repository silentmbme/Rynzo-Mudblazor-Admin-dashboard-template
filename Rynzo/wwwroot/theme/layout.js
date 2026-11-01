(() => {
    const shortcut = event => {
        if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'k') {
            event.preventDefault(); document.getElementById('rynzo-global-search')?.click();
        }
    };
    window.rynzoLayout = {
        connect() { document.addEventListener('keydown', shortcut); },
        disconnect() { document.removeEventListener('keydown', shortcut); },
        async fullscreen() {
            try {
                if (document.fullscreenElement) await document.exitFullscreen();
                else await document.documentElement.requestFullscreen();
                return true;
            } catch { return false; }
        }
    };
})();
