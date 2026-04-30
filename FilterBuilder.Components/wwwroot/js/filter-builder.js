window.FilterBuilder = {
    getRect: function (el) {
        const r = el.getBoundingClientRect();
        return { top: r.top, left: r.left, bottom: r.bottom, right: r.right, width: r.width };
    }
};
