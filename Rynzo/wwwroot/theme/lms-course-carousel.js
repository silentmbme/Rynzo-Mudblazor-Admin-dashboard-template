import Swiper from '../vendor/swiper/swiper-bundle.min.js';

const instances = new Map();
export function render(root) {
    // Blazor replaces the keyed carousel after filters change; discard detached instances.
    for (const element of instances.keys()) if (!element.isConnected || element === root) dispose(element);
    const container = root.querySelector('.catalog-swiper');
    if (!container) return;
    const reduceMotion = matchMedia('(prefers-reduced-motion: reduce)').matches;
    const swiper = new Swiper(container, {
        slidesPerView: 1,
        spaceBetween: 18,
        breakpointsBase: 'container',
        breakpoints: { 540: { slidesPerView: 2 }, 960: { slidesPerView: 3 } },
        speed: reduceMotion ? 0 : 350,
        loop: false,
        watchOverflow: true,
        keyboard: { enabled: true, onlyInViewport: true, pageUpDown: false },
        navigation: { prevEl: root.querySelector('.catalog-prev'), nextEl: root.querySelector('.catalog-next') },
        pagination: { el: root.querySelector('.catalog-pagination'), clickable: true },
        a11y: { enabled: true, containerRole: 'region', containerMessage: 'Course catalog', itemRoleDescriptionMessage: 'course', prevSlideMessage: 'Previous courses', nextSlideMessage: 'Next courses' },
        on: {
            init: updateFocus,
            slideChange: updateFocus,
            resize: updateFocus,
        },
    });
    instances.set(root, swiper);
}
function updateFocus(swiper) {
    // Keep offscreen course actions out of the keyboard sequence.
    requestAnimationFrame(() => {
        if (swiper.destroyed) return;
        const count = Math.ceil(Number(swiper.params.slidesPerView));
        swiper.slides.forEach((slide, index) => {
            const visible = index >= swiper.activeIndex && index < swiper.activeIndex + count;
            slide.inert = !visible;
            slide.setAttribute('aria-hidden', String(!visible));
        });
    });
}
export function dispose(root) {
    const swiper = instances.get(root);
    if (swiper) { swiper.destroy(true, true); instances.delete(root); }
}
