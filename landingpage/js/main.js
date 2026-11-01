(() => {
  const header = document.querySelector(".site-header");
  const menuButton = document.querySelector(".menu-toggle");
  const nav = document.querySelector(".primary-nav");

  const closeMenu = () => {
    nav?.classList.remove("is-open");
    menuButton?.setAttribute("aria-expanded", "false");
    menuButton?.setAttribute("aria-label", "Open navigation");
    document.body.classList.remove("nav-open");
  };

  menuButton?.addEventListener("click", () => {
    const open = menuButton.getAttribute("aria-expanded") !== "true";
    menuButton.setAttribute("aria-expanded", String(open));
    menuButton.setAttribute("aria-label", open ? "Close navigation" : "Open navigation");
    nav?.classList.toggle("is-open", open);
    document.body.classList.toggle("nav-open", open);
  });
  nav?.querySelectorAll("a").forEach((link) => link.addEventListener("click", closeMenu));
  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") closeMenu();
  });

  const localApp = "http://localhost:5245";
  const isLocalHost = ["localhost", "127.0.0.1", ""].includes(location.hostname);
  const appOrigin = location.protocol === "file:" || isLocalHost ? localApp : location.origin;
  document.querySelectorAll("[data-app-route]").forEach((link) => {
    const route = link.getAttribute("href");
    if (route?.startsWith("/")) link.href = new URL(route, appOrigin).href;
  });

  document.querySelectorAll("[data-filter]").forEach((button) => {
    button.addEventListener("click", () => {
      const filter = button.dataset.filter;
      document.querySelectorAll("[data-filter]").forEach((item) => {
        const active = item === button;
        item.classList.toggle("is-active", active);
        item.setAttribute("aria-pressed", String(active));
      });
      document.querySelectorAll(".dashboard-card[data-category]").forEach((card) => {
        card.hidden = filter !== "all" && card.dataset.category !== filter;
      });
    });
  });

  const customizer = document.querySelector(".customizer");
  const preview = document.querySelector(".customizer-preview");
  document.querySelectorAll("[data-skin-choice]").forEach((button) => {
    button.addEventListener("click", () => {
      customizer?.setAttribute("data-skin", button.dataset.skinChoice);
      document.querySelectorAll("[data-skin-choice]").forEach((item) => {
        const active = item === button;
        item.classList.toggle("is-active", active);
        item.setAttribute("aria-pressed", String(active));
      });
    });
  });
  document.querySelectorAll("[data-mode-choice]").forEach((button) => {
    button.addEventListener("click", () => {
      const dark = button.dataset.modeChoice === "dark";
      preview?.classList.toggle("is-dark", dark);
      document.querySelectorAll("[data-mode-choice]").forEach((item) => {
        const active = item === button;
        item.classList.toggle("is-active", active);
        item.setAttribute("aria-pressed", String(active));
      });
    });
  });

  const reveal = document.querySelectorAll("[data-reveal]");
  if ("IntersectionObserver" in window && !window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
    reveal.forEach((element) => element.classList.add("reveal-pending"));
    const observer = new IntersectionObserver((entries, currentObserver) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          entry.target.classList.add("is-visible");
          currentObserver.unobserve(entry.target);
        }
      });
    }, { rootMargin: "0px 0px -40px 0px", threshold: 0.08 });
    reveal.forEach((element) => observer.observe(element));
  }

  const syncHeader = () => header?.classList.toggle("is-scrolled", window.scrollY > 8);
  window.addEventListener("scroll", syncHeader, { passive: true });
  syncHeader();

  const year = document.querySelector("[data-current-year]");
  if (year) year.textContent = String(new Date().getFullYear());
})();
