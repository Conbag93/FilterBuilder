namespace FilterBuilder.Components;

/// <summary>
/// Optional style overrides for <see cref="ConditionEditor"/>. Pass via the <c>Theme</c>
/// parameter; it cascades automatically to all child components.
///
/// Nullable properties replace the built-in fb-* default when set.
/// <c>RootClass</c> is the only append-only property (always added alongside fb-root).
/// Icon properties replace the default SVG with a &lt;span class="…"&gt; when set.
/// </summary>
public record FilterBuilderTheme
{
    // ── Container ─────────────────────────────────────────────────────────────

    /// Appended to the structural <c>fb-root</c> element.
    public string RootClass { get; init; } = "";

    // ── Pill modifier classes ─────────────────────────────────────────────────
    // null → use the built-in fb-pill--* class; non-null → replace it entirely.
    // The structural fb-pill class is always applied by PillDropdown.

    public string? GroupPillClass { get; init; }
    public string? FieldPillClass { get; init; }
    public string? OperatorPillClass { get; init; }
    public string? ValuePillClass { get; init; }

    // ── Value input ───────────────────────────────────────────────────────────

    /// Replaces <c>fb-value-input</c> on text and number inputs when set.
    public string? ValueInputClass { get; init; }

    // ── Button modifier classes ───────────────────────────────────────────────

    public string? AddButtonClass { get; init; }
    public string? DeleteButtonClass { get; init; }

    // ── Dropdown ──────────────────────────────────────────────────────────────

    public string? DropdownPanelClass { get; init; }
    public string? DropdownItemClass { get; init; }
    public string? DropdownItemSelectedClass { get; init; }

    // ── Icon overrides ────────────────────────────────────────────────────────
    // When set, renders <span class="…"></span> instead of the built-in SVG.
    // Example: DragHandleIcon = "icon-drag icon-small"

    public string? DragHandleIcon { get; init; }
    public string? AddIcon { get; init; }
    public string? DeleteIcon { get; init; }
    public string? ChevronIcon { get; init; }
    public string? ConditionMenuIcon { get; init; }
    public string? GroupMenuIcon { get; init; }

    public static FilterBuilderTheme Default { get; } = new();
}
