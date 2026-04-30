# FilterBuilder.Components

A Blazor condition/filter builder with no external UI dependencies. Supports drag-and-drop reordering, nested groups, custom themes, and extensible field/operator/value types.

![FilterBuilder demo](drag-drop-working.png)

**[Live demo →](https://conbag93.github.io/FilterBuilder/)**

## Quick start

### 1. Install

```
dotnet add package FilterBuilder.Components
```

### 2. Add CSS and JS

**Blazor Web App** — in `App.razor`, inside `<head>`:

```html
<link rel="stylesheet" href="_content/FilterBuilder.Components/css/filter-builder.css" />
```

And before `</body>`:

```html
<script src="_content/FilterBuilder.Components/js/filter-builder.js"></script>
```

**Blazor WASM** — same lines in `wwwroot/index.html`.

### 3. Use the component

```razor
@using FilterBuilder.Components

<ConditionEditor Condition="@_condition"
                 Fields="@_fields"
                 ConditionChanged="@(c => _condition = c)" />

@code {
    private static readonly IReadOnlyList<FieldDef> _fields =
    [
        new FieldDef("Name",     "Name",     FactValueKind.Text),
        new FieldDef("Category", "Category", FactValueKind.Text),
        new FieldDef("Price",    "Price",    FactValueKind.Number),
        new FieldDef("InStock",  "In Stock", FactValueKind.Bool),
    ];

    private Condition _condition = new Condition.All([]);
}
```

> **Blazor Server / interactive render mode** — add `@rendermode InteractiveServer` to the page or component.

No DI registration required.

### Optional node labels

Any node can be given a human-readable name via the `Label` property. A labelled node renders as a collapsed pill; clicking the chevron expands the full detail view. Labels round-trip through `JsonSerializer` with no extra setup.

```csharp
private Condition _condition = new Condition.All(
[
    new Condition.All([ /* … */ ]) { Label = "normal medium-value Q1 spend" },
    new Condition.Atom("category", Operator.Neq, new FactValue.Text("internal")),
]);
```

To set a label interactively: open the `+` menu on any group and choose **Set label**, or hover an atom row and click the tag icon.
