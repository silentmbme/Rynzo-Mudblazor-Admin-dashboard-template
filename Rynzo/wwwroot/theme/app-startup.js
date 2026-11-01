(() => {
    const panel = document.getElementById('rynzo-startup');
    const message = panel.querySelector('[data-startup-message]');
    const reload = panel.querySelector('[data-startup-reload]');
    reload.addEventListener('click', () => window.location.reload());
    let timer;
    const observer = new MutationObserver(finish);
    function finish() {
        if (!document.querySelector('.rynzo-layout, .auth-layout, #blazor-error-ui')) return;
        clearTimeout(timer);
        observer.disconnect();
        panel.remove();
    }
    observer.observe(document.body, { childList: true, subtree: true });
    timer = setTimeout(() => {
        message.textContent = 'Your workspace is taking longer to load. Check your connection, or reload to try again.';
        reload.hidden = false;
    }, 20000);
    Promise.resolve().then(() => Blazor.start()).then(() => {
        // Successful startup can render authentication layouts as well as the app shell.
        clearTimeout(timer);
        observer.disconnect();
        panel.remove();
    }).catch(error => {
        clearTimeout(timer);
        observer.disconnect();
        panel.setAttribute('role', 'alert');
        panel.querySelector('h1').textContent = 'Rynzo could not start';
        message.textContent = 'The workspace could not finish loading. Reload to try again.';
        reload.hidden = false;
        console.error('Rynzo startup failed:', error);
    });
})();
