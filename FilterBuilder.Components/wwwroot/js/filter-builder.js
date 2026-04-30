window.FilterBuilder = {
    getRect: function (el) {
        const r = el.getBoundingClientRect();
        return { top: r.top, left: r.left, bottom: r.bottom, right: r.right, width: r.width };
    },

    // Lazy-created fixed indicator line appended to <body> once
    _indicator: null,

    _getIndicator: function () {
        if (!FilterBuilder._indicator) {
            const el = document.createElement('div');
            el.className = 'fb-drop-indicator';
            document.body.appendChild(el);
            FilterBuilder._indicator = el;
        }
        return FilterBuilder._indicator;
    },

    _showIndicator: function (rect, above) {
        const ind = FilterBuilder._getIndicator();
        const y = above ? rect.top : rect.bottom;
        ind.style.top = (y - 1) + 'px';
        ind.style.left = rect.left + 'px';
        ind.style.width = rect.width + 'px';
        ind.style.display = 'block';
    },

    _hideIndicator: function () {
        if (FilterBuilder._indicator) {
            FilterBuilder._indicator.style.display = 'none';
        }
    },

    // Wire up HTML5 drag-and-drop event delegation on the fb-root container.
    // All visual feedback runs here in JS (no Blazor re-renders on dragover).
    // Only drop calls back to Blazor via dotnetRef.invokeMethodAsync('OnDrop', ...).
    initDrag: function (container, dotnetRef) {
        let dragSource = null;  // { groupPath: int[], index: int }
        let lastHoverRow = null;
        let dropAbove = true;

        function clearHover() {
            if (lastHoverRow) {
                lastHoverRow.classList.remove('fb-drop-above', 'fb-drop-below');
                lastHoverRow = null;
            }
            FilterBuilder._hideIndicator();
        }

        // Returns true if `row` is inside the currently-dragged node
        // (prevents dropping a group into its own descendant).
        function isInsideDragSource(row) {
            if (!dragSource) return false;
            const prefix = [...dragSource.groupPath, dragSource.index].join(',');
            const rowPath = row.dataset.fbGroupPath
                ? JSON.parse(row.dataset.fbGroupPath)
                : [];
            const rowPrefix = rowPath.slice(0, dragSource.groupPath.length + 1).join(',');
            return prefix !== '' && rowPrefix.startsWith(prefix) &&
                (rowPrefix === prefix || rowPrefix[prefix.length] === ',');
        }

        function validRow(el) {
            return el && parseInt(el.dataset.fbIndex, 10) >= 0;
        }

        container.addEventListener('dragstart', function (e) {
            const row = e.target.closest('[data-fb-index]');
            if (!validRow(row)) return;
            e.dataTransfer.effectAllowed = 'move';
            // Required for Firefox to start the drag
            e.dataTransfer.setData('text/plain', '');
            row.classList.add('fb-row--dragging');
            dragSource = {
                groupPath: JSON.parse(row.dataset.fbGroupPath || '[]'),
                index: parseInt(row.dataset.fbIndex, 10)
            };
        });

        container.addEventListener('dragend', function () {
            container.querySelectorAll('.fb-row--dragging').forEach(function (el) {
                el.classList.remove('fb-row--dragging');
            });
            clearHover();
            dragSource = null;
        });

        container.addEventListener('dragover', function (e) {
            if (!dragSource) return;
            const row = e.target.closest('[data-fb-index]');
            if (!validRow(row)) return;
            if (row.classList.contains('fb-row--dragging')) return;
            if (isInsideDragSource(row)) return;

            e.preventDefault();  // required to allow drop

            if (lastHoverRow && lastHoverRow !== row) {
                lastHoverRow.classList.remove('fb-drop-above', 'fb-drop-below');
            }

            const rect = row.getBoundingClientRect();
            dropAbove = e.clientY < rect.top + rect.height * 0.5;
            row.classList.remove('fb-drop-above', 'fb-drop-below');
            row.classList.add(dropAbove ? 'fb-drop-above' : 'fb-drop-below');
            lastHoverRow = row;

            FilterBuilder._showIndicator(rect, dropAbove);
        });

        container.addEventListener('dragleave', function (e) {
            // Only clear when leaving the container entirely
            if (!container.contains(e.relatedTarget)) {
                clearHover();
            }
        });

        container.addEventListener('drop', function (e) {
            if (!dragSource) return;
            const row = e.target.closest('[data-fb-index]');
            if (!validRow(row)) return;
            if (isInsideDragSource(row)) return;

            e.preventDefault();

            const dstGroupPath = JSON.parse(row.dataset.fbGroupPath || '[]');
            const dstIndex = parseInt(row.dataset.fbIndex, 10);
            const above = dropAbove;

            const src = dragSource;
            dragSource = null;
            clearHover();
            container.querySelectorAll('.fb-row--dragging').forEach(function (el) {
                el.classList.remove('fb-row--dragging');
            });

            dotnetRef.invokeMethodAsync('OnDrop',
                src.groupPath, src.index,
                dstGroupPath, dstIndex,
                above
            );
        });
    }
};
