// Capture current Rynzo browser previews for the static landing page.
// Requires the app at http://localhost:5245 and Google Chrome on Windows.
const { spawn } = require('node:child_process');
const fs = require('node:fs/promises');
const path = require('node:path');
const os = require('node:os');

const root = path.resolve(__dirname, '..');
const output = path.join(root, 'landingpage/assets/screenshots');
const chrome = process.env.CHROME_PATH || 'C:/Program Files/Google/Chrome/Application/chrome.exe';
const viewport = { width: 1440, height: 1000 };
const appOrigin = 'http://localhost:5245';
const dashboardPages = [
  ['Executive', '/', 'dashboard-executive-v2.png'],
  ['Analytics', '/dashboards/analytics', 'dashboard-analytics-v2.png'],
  ['eCommerce', '/dashboards/ecommerce', 'dashboard-ecommerce-v2.png'],
  ['AI Analytics', '/dashboards/ai-analytics', 'dashboard-ai-analytics-v2.png'],
  ['Crypto', '/dashboards/crypto', 'dashboard-crypto-v2.png'],
  ['Hospital', '/dashboards/hospital-management', 'dashboard-hospital-management-v2.png'],
  ['LMS', '/dashboards/lms-dashboard', 'dashboard-lms-dashboard-v2.png'],
  ['CRM', '/dashboards/crm', 'dashboard-crm-v2.png'],
  ['Logistics', '/dashboards/logistics-dashboard', 'dashboard-logistics-dashboard-v2.png'],
  ['Projects', '/dashboards/projects', 'dashboard-projects-v2.png'],
  ['HRM', '/dashboards/hrm-dashboard', 'dashboard-hrm-dashboard-v2.png'],
  ['School', '/dashboards/school-management', 'dashboard-school-management-v2.png'],
  ['Restaurant POS', '/dashboards/restaurant-pos', 'dashboard-restaurant-pos-v2.png'],
  ['Finance', '/dashboards/finance', 'dashboard-finance-v2.png'],
  ['Marketing', '/dashboards/marketing', 'dashboard-marketing-v2.png'],
  ['SaaS', '/dashboards/saas', 'dashboard-saas-v2.png'],
];
const appPages = [
  ['Email', '/apps/email', 'app-email.png'],
  ['Chat', '/apps/chat', 'app-chat.png'],
  ['Calendar', '/apps/calendar', 'app-calendar.png'],
  ['Tasks', '/apps/tasks', 'app-tasks.png'],
  ['Kanban', '/apps/kanban', 'app-kanban.png'],
  ['File Manager', '/apps/files', 'app-files.png'],
  ['AI Chat', '/apps/ai-chat', 'app-ai-chat-v2.png'],
  ['Notes', '/apps/notes', 'app-notes-v2.png'],
];

const delay = ms => new Promise(resolve => setTimeout(resolve, ms));
async function waitForFile(file, timeout = 12000) {
  const end = Date.now() + timeout;
  while (Date.now() < end) {
    try { return await fs.readFile(file, 'utf8'); } catch { await delay(100); }
  }
  throw new Error(`Chrome did not create ${file}`);
}

class DevTools {
  constructor(socket) {
    this.socket = socket;
    this.nextId = 0;
    this.pending = new Map();
    socket.addEventListener('message', event => {
      const message = JSON.parse(event.data);
      if (!message.id) return;
      const pending = this.pending.get(message.id);
      if (!pending) return;
      this.pending.delete(message.id);
      message.error ? pending.reject(new Error(message.error.message)) : pending.resolve(message.result);
    });
  }
  send(method, params = {}) {
    const id = ++this.nextId;
    return new Promise((resolve, reject) => {
      this.pending.set(id, { resolve, reject });
      this.socket.send(JSON.stringify({ id, method, params }));
    });
  }
}

async function capturePage(devtools, [title, route, filename]) {
  let load;
  const onMessage = event => {
    const message = JSON.parse(event.data);
    if (message.method === 'Page.loadEventFired' && load) {
      load.resolve();
      load = null;
    }
  };
  devtools.socket.addEventListener('message', onMessage);
  const loaded = new Promise((resolve, reject) => {
    const timeout = setTimeout(() => reject(new Error(`Timed out loading ${route}`)), 35000);
    load = { resolve: () => { clearTimeout(timeout); resolve(); } };
  });
  await devtools.send('Page.navigate', { url: new URL(route, appOrigin).href });
  await loaded;

  const end = Date.now() + 35000;
  let ready = false;
  while (Date.now() < end) {
    const result = await devtools.send('Runtime.evaluate', {
      expression: `(() => { const layout = document.querySelector('.rynzo-layout'); const text = document.body?.innerText || ''; return !!layout && text.trim().length > 160 && !text.includes('Opening Rynzo'); })()`,
      returnByValue: true,
    });
    if (result.result?.value) { ready = true; break; }
    await delay(350);
  }
  devtools.socket.removeEventListener('message', onMessage);
  if (!ready) throw new Error(`Rynzo did not render ${route}`);

  await devtools.send('Runtime.evaluate', {
    expression: 'document.fonts.ready.then(() => new Promise(resolve => setTimeout(resolve, 900)))',
    awaitPromise: true,
  });
  const shot = await devtools.send('Page.captureScreenshot', {
    format: 'png', fromSurface: true, captureBeyondViewport: false,
  });
  await fs.writeFile(path.join(output, filename), Buffer.from(shot.data, 'base64'));
  console.log(`Captured ${title}: ${route}`);
  return { title, route, filename };
}

async function main() {
  await fs.mkdir(output, { recursive: true });
  const profile = await fs.mkdtemp(path.join(os.tmpdir(), 'rynzo-demo-capture-'));
  const child = spawn(chrome, [
    '--headless=new', '--disable-gpu', '--no-first-run', '--no-default-browser-check',
    '--disable-extensions', '--hide-scrollbars', '--force-device-scale-factor=1',
    '--remote-debugging-port=0', `--user-data-dir=${profile}`, 'about:blank',
  ], { windowsHide: true, stdio: 'ignore' });

  let socket;
  try {
    const portInfo = await waitForFile(path.join(profile, 'DevToolsActivePort'));
    const port = Number(portInfo.split(/\r?\n/)[0]);
    const targetResponse = await fetch(`http://127.0.0.1:${port}/json/new?about%3Ablank`, { method: 'PUT' });
    if (!targetResponse.ok) throw new Error(`Chrome DevTools target request failed: ${targetResponse.status}`);
    const target = await targetResponse.json();
    socket = new WebSocket(target.webSocketDebuggerUrl);
    await new Promise((resolve, reject) => {
      socket.addEventListener('open', resolve, { once: true });
      socket.addEventListener('error', reject, { once: true });
    });
    const devtools = new DevTools(socket);
    await devtools.send('Page.enable');
    await devtools.send('Runtime.enable');
    await devtools.send('Emulation.setDeviceMetricsOverride', {
      width: viewport.width, height: viewport.height, deviceScaleFactor: 1, mobile: false,
    });
    await devtools.send('Emulation.setEmulatedMedia', { features: [{ name: 'prefers-color-scheme', value: 'light' }] });

    const dashboards = [];
    const apps = [];
    for (const page of dashboardPages) dashboards.push(await capturePage(devtools, page));
    for (const page of appPages) apps.push(await capturePage(devtools, page));
    const manifest = {
      source: appOrigin,
      method: 'Chrome DevTools Protocol capture from the current Rynzo .NET/Blazor app',
      capturedAt: new Date().toISOString(),
      viewport,
      dashboards,
      apps,
    };
    await fs.writeFile(path.join(output, 'dashboard-captures.json'), `${JSON.stringify({ ...manifest, entries: dashboards }, null, 2)}\n`);
    await fs.writeFile(path.join(output, 'app-captures.json'), `${JSON.stringify({ ...manifest, entries: apps }, null, 2)}\n`);
    console.log(`Captured ${dashboards.length} dashboards and ${apps.length} apps.`);
  } finally {
    socket?.close();
    child.kill();
  }
}

main().catch(error => {
  console.error(error.stack || error);
  process.exitCode = 1;
});
