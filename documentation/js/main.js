(() => {
  const sidebar = document.querySelector(".sidebar");
  const overlay = document.querySelector(".overlay");
  const toggle = document.querySelector(".menu-toggle");
  const search = document.querySelector(".search");
  const empty = document.querySelector(".search-empty");
  const top = document.querySelector(".to-top");
  const closeMenu = () => {
    sidebar?.classList.remove("open");
    overlay?.classList.remove("visible");
  };

  toggle?.addEventListener("click", () => {
    sidebar?.classList.toggle("open");
    overlay?.classList.toggle("visible");
    toggle.setAttribute("aria-expanded", String(sidebar?.classList.contains("open")));
  });
  overlay?.addEventListener("click", closeMenu);

  // Keep preview cards usable from a directly opened documentation file.
  // Production links use the current host; local files open the Rynzo app.
  const demoOrigin =
    location.protocol === "file:" ? "http://localhost:5245" : location.origin;
  document
    .querySelectorAll(
      ".preview-card[href^='/'], [data-live-preview][href^='/']",
    )
    .forEach((card) => {
      const route = card.getAttribute("href");
      if (route) card.href = new URL(route, demoOrigin).href;
    });

  document.querySelectorAll(".nav-link").forEach((link) => {
    link.addEventListener("click", () => {
      document
        .querySelectorAll(".nav-link.active")
        .forEach((item) => item.classList.remove("active"));
      link.classList.add("active");
      closeMenu();
    });
  });

  search?.addEventListener("input", (event) => {
    const query = event.target.value.toLowerCase().trim();
    let matches = 0;
    document.querySelectorAll(".nav-link").forEach((link) => {
      const text =
        `${link.textContent} ${link.dataset.keywords || ""}`.toLowerCase();
      const visible = !query || text.includes(query);
      link.style.display = visible ? "block" : "none";
      if (visible) matches += 1;
    });
    if (empty) empty.style.display = matches ? "none" : "block";
  });

  document.querySelectorAll(".copy").forEach((button) => {
    button.addEventListener("click", async () => {
      const code = button.parentElement.querySelector("code").innerText;
      try {
        await navigator.clipboard.writeText(code);
        button.textContent = "Copied";
        setTimeout(() => {
          button.textContent = "Copy";
        }, 1200);
      } catch {
        button.textContent = "Select code";
      }
    });
  });

  window.addEventListener("scroll", () =>
    top?.classList.toggle("visible", window.scrollY > 300),
  );
  top?.addEventListener("click", () =>
    window.scrollTo({ top: 0, behavior: "smooth" }),
  );
  const normalizePath = (path) => {
    const normalized = path
      .replace(/\\/g, "/")
      .replace(/\/$/, "")
      .replace(/\.html$/, "");
    return normalized.endsWith("/index") || !normalized ? "/" : normalized;
  };
  const currentPath = normalizePath(location.pathname);
  document.querySelectorAll(".nav-link").forEach((link) => {
    const linkPath = normalizePath(
      new URL(link.getAttribute("href"), location.href).pathname,
    );
    link.classList.toggle("active", linkPath === currentPath);
  });
})();

(() => {
  const input = document.querySelector("#route-search");
  const rows = [...document.querySelectorAll(".route-table tbody tr")];
  const groups = [...document.querySelectorAll(".route-group")];
  const filter = () => {
    const query = input.value.trim().toLowerCase();
    let count = 0;
    for (const row of rows) {
      row.hidden = !row.textContent.toLowerCase().includes(query);
      if (!row.hidden) count++;
    }
    for (const group of groups) {
      group.hidden = !group.querySelector("tbody tr:not([hidden])");
    }
    const status = document.querySelector("#route-count");
    if (status) status.textContent = `${count} of ${rows.length} routes`;
  };
  input?.addEventListener("input", filter);
  if (input) filter();
  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") {
      document.querySelector(".sidebar")?.classList.remove("open");
      document.querySelector(".overlay")?.classList.remove("visible");
    }
  });
})();

// Shared reading navigation, without changing guide content.
(() => {
  const content = document.querySelector(".content");
  const shell = document.querySelector(".shell");
  if (!content || !shell) return;
  const title = content.querySelector("h1")?.textContent || "Documentation";
  const breadcrumb = document.createElement("div");
  breadcrumb.className = "doc-breadcrumb";
  const home = document.createElement("a");
  home.href = "index.html";
  home.textContent = "Documentation";
  const separator = document.createElement("span");
  separator.textContent = "/";
  const current = document.createElement("span");
  current.textContent = title;
  breadcrumb.append(home, separator, current);
  content.prepend(breadcrumb);
  const headings = [...content.querySelectorAll("h2")];
  const used = new Set(
    [...document.querySelectorAll("[id]")].map((el) => el.id),
  );
  const toc = document.createElement("nav");
  toc.className = "doc-toc";
  toc.setAttribute("aria-label", "On this page");
  const caption = document.createElement("p");
  caption.className = "doc-toc-title";
  caption.textContent = "On this page";
  toc.append(caption);
  headings.forEach((heading, index) => {
    if (!heading.id) {
      const base =
        heading.textContent
          .trim()
          .toLowerCase()
          .replace(/[^a-z0-9]+/g, "-")
          .replace(/^-|-$/g, "") || "section";
      let id = base;
      let suffix = 2;
      while (used.has(id)) id = `${base}-${suffix++}`;
      heading.id = id;
      used.add(id);
    }
    const link = document.createElement("a");
    link.href = `#${heading.id}`;
    link.textContent = heading.textContent;
    toc.append(link);
  });
  const note = document.createElement("div");
  note.className = "doc-toc-note";
  const label = document.createElement("strong");
  label.textContent = "Rynzo workspace";
  const detail = document.createElement("span");
  detail.textContent = "Setup, customization, and workspace guides.";
  note.append(label, detail);
  toc.append(note);
  shell.append(toc);
  const tocLinks = [...toc.querySelectorAll("a")];
  const progress = document.createElement("div");
  progress.className = "reading-progress";
  progress.setAttribute("aria-hidden", "true");
  document.body.append(progress);
  let scheduled = false;
  const update = () => {
    scheduled = false;
    const range = document.documentElement.scrollHeight - innerHeight;
    progress.style.width = `${range > 0 ? Math.min(100, (scrollY / range) * 100) : 100}%`;
    let active = 0;
    headings.forEach((heading, index) => {
      if (heading.getBoundingClientRect().top <= 145) active = index;
    });
    tocLinks.forEach((link, index) => {
      link.classList.toggle("active", index === active);
      if (index === active) link.setAttribute("aria-current", "location");
      else link.removeAttribute("aria-current");
    });
  };
  window.addEventListener(
    "scroll",
    () => {
      if (!scheduled) {
        scheduled = true;
        requestAnimationFrame(update);
      }
    },
    { passive: true },
  );
  window.addEventListener("resize", update);
  update();
  const links = [...document.querySelectorAll(".sidebar .nav-link")].filter(
    (link) => !link.getAttribute("href").includes("#"),
  );
  const index = links.findIndex(
    (link) => new URL(link.href).pathname === location.pathname,
  );
  if (index >= 0) {
    const pagination = document.createElement("nav");
    pagination.className = "doc-pagination";
    pagination.setAttribute("aria-label", "Guide navigation");
    [
      index > 0 ? links[index - 1] : null,
      index < links.length - 1 ? links[index + 1] : null,
    ].forEach((link, side) => {
      if (!link) {
        pagination.append(document.createElement("span"));
        return;
      }
      const target = document.createElement("a");
      target.href = link.getAttribute("href");
      const hint = document.createElement("small");
      hint.textContent = side ? "Next guide →" : "← Previous guide";
      const name = document.createElement("span");
      name.textContent = link.textContent.trim();
      target.append(hint, name);
      pagination.append(target);
    });
    const footer = content.querySelector(".footer-note");
    if (footer) content.insertBefore(pagination, footer);
    else content.append(pagination);
  }
  const iconPaths = [
    "M4 4h16v16H4z M8 4v16 M8 9h12",
    "M8 3H5a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-3 M16 3h5v5 M10 14 21 3",
    "M4 7h16v14H4z M8 3h8v4 M8 12h8 M8 16h5",
  ];
  document.querySelectorAll(".sidebar .nav-link").forEach((link, index) => {
    const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
    svg.setAttribute("viewBox", "0 0 24 24");
    svg.setAttribute("fill", "none");
    svg.setAttribute("stroke", "currentColor");
    svg.setAttribute("stroke-width", "1.6");
    svg.setAttribute("stroke-linecap", "round");
    svg.setAttribute("stroke-linejoin", "round");
    svg.setAttribute("aria-hidden", "true");
    const path = document.createElementNS("http://www.w3.org/2000/svg", "path");
    path.setAttribute("d", iconPaths[index % iconPaths.length]);
    svg.append(path);
    link.prepend(svg);
    const url = new URL(link.href);
    const active =
      url.pathname === location.pathname &&
      (!url.hash || url.hash === location.hash);
    link.classList.toggle("active", active);
    if (active) link.setAttribute("aria-current", "page");
    else link.removeAttribute("aria-current");
  });
  content.querySelectorAll("table").forEach((table) => {
    if (
      !table.parentElement.classList.contains("docs-table") &&
      !table.parentElement.classList.contains("table-wrap")
    ) {
      const wrapper = document.createElement("div");
      wrapper.className = "docs-table";
      table.before(wrapper);
      wrapper.append(table);
    }
  });
  const toggle = document.querySelector(".menu-toggle");
  const sidebar = document.querySelector(".sidebar");
  if (sidebar) {
    sidebar.id = "documentation-sidebar";
    toggle?.setAttribute("aria-controls", sidebar.id);
    toggle?.setAttribute("aria-expanded", "false");
    new MutationObserver(() =>
      toggle?.setAttribute(
        "aria-expanded",
        String(sidebar.classList.contains("open")),
      ),
    ).observe(sidebar, { attributes: true, attributeFilter: ["class"] });
  }
})();

// Release filters leave the original release history visible.
(() => {
  const controls = [...document.querySelectorAll("[data-release-filter]")];
  const groups = [...document.querySelectorAll("[data-release-type]")];
  const status = document.querySelector(".release-status");
  controls.forEach((button) =>
    button.addEventListener("click", () => {
      const selected = button.dataset.releaseFilter;
      controls.forEach((control) => {
        const active = control === button;
        control.classList.toggle("active", active);
        control.setAttribute("aria-pressed", String(active));
      });
      let count = 0;
      groups.forEach((group) => {
        group.hidden =
          selected !== "All" && selected !== group.dataset.releaseType;
        if (!group.hidden) count += group.querySelectorAll("li").length;
      });
      if (status)
        status.textContent =
          selected === "All"
            ? `Showing all ${count} updates`
            : `Showing ${count} ${selected.toLowerCase()} updates`;
    }),
  );
})();
