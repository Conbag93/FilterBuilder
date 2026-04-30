# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="0.1.0"></a>
## [0.1.0](https://www.github.com/Conbag93/FilterBuilder/releases/tag/v0.1.0) (2026-04-30)

### Features

* add optional node labels with collapse/expand UX (v0.1.0) ([a1de0c4](https://www.github.com/Conbag93/FilterBuilder/commit/a1de0c4fdaf137bf9b3935e4f9f0ddaac32531f0))

<a name="0.1.0"></a>
## [0.1.0] (2026-04-30)

### Features

- **Node labels** — every node (`Condition.Atom`, `All`, `Any`, `Not`) now has an optional `string? Label` property. When set, the node renders as a single amber pill showing the label text; clicking the chevron expands the full detail view, and clicking again collapses it. The label is purely presentational — two nodes with the same label are completely independent, and the library has no name registry.
- **Authoring** — groups gain a *Set label* item in the `+` dropdown; atoms gain a hover-visible tag icon button. Both open an inline input; `Enter` commits, `Escape` cancels. Clearing the text removes the label.
- **Round-trip serialisation** — `Label` is an `init`-only property on the abstract `Condition` record and survives `JsonSerializer` (de)serialisation with no extra configuration.
- **Drag-and-drop** — the label travels with its node; drag-drop behaviour is unchanged.
- **Theme slot** — `FilterBuilderTheme.LabelPillClass` replaces the built-in `fb-pill--label` class when set.

<a name="0.0.1"></a>
## [0.0.1](https://www.github.com/Conbag93/FilterBuilder/releases/tag/v0.0.1) (2026-04-30)

<a name="0.0.0"></a>
## [0.0.0](https://www.github.com/Conbag93/FilterBuilder/releases/tag/v0.0.0) (2026-04-30)

