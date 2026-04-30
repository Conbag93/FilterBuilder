namespace FilterBuilder.Components;

// ── Operator ──────────────────────────────────────────────────────────────────

public enum Operator
{
    Eq, Neq,
    Gt, Gte, Lt, Lte,
    In,
    IsNull, IsNotNull,
    BeginsWith, EndsWith, Contains, DoesNotContain,
}

// ── FactValue ─────────────────────────────────────────────────────────────────

public abstract record FactValue
{
    public sealed record Text(string Value) : FactValue;
    public sealed record Number(decimal Value) : FactValue;
    public sealed record Bool(bool Value) : FactValue;
}

// ── Condition ─────────────────────────────────────────────────────────────────

public abstract record Condition
{
    /// Human-readable name for this node. When set, the node renders as a collapsed pill;
    /// clicking the chevron expands the detail view. Null means no label (normal rendering).
    public string? Label { get; init; }

    public sealed record Atom(string Field, Operator Op, FactValue? Value = null) : Condition;
    public sealed record All(IReadOnlyList<Condition> Children) : Condition;
    public sealed record Any(IReadOnlyList<Condition> Children) : Condition;
    public sealed record Not(Condition Inner) : Condition;
}

// ── Field schema ──────────────────────────────────────────────────────────────

public enum FactValueKind { Text, Number, Bool }

public record FieldDef(string Name, string Caption, FactValueKind Kind);

// ── Extensions ────────────────────────────────────────────────────────────────

public static class ConditionExtensions
{
    public static string Label(this Operator op) => op switch
    {
        Operator.Eq             => "=",
        Operator.Neq            => "≠",
        Operator.Gt             => ">",
        Operator.Gte            => "≥",
        Operator.Lt             => "<",
        Operator.Lte            => "≤",
        Operator.In             => "in",
        Operator.IsNull         => "is blank",
        Operator.IsNotNull      => "is not blank",
        Operator.BeginsWith     => "begins with",
        Operator.EndsWith       => "ends with",
        Operator.Contains       => "contains",
        Operator.DoesNotContain => "does not contain",
        _                       => op.ToString(),
    };

    public static bool NeedsValue(this Operator op) =>
        op is not (Operator.IsNull or Operator.IsNotNull);

    public static IReadOnlyList<Operator> ValidOperators(this FactValueKind kind) => kind switch
    {
        FactValueKind.Text   => new[] { Operator.Eq, Operator.Neq, Operator.BeginsWith, Operator.EndsWith, Operator.Contains, Operator.DoesNotContain, Operator.In, Operator.IsNull, Operator.IsNotNull },
        FactValueKind.Number => new[] { Operator.Eq, Operator.Neq, Operator.Gt, Operator.Gte, Operator.Lt, Operator.Lte, Operator.IsNull, Operator.IsNotNull },
        FactValueKind.Bool   => new[] { Operator.Eq, Operator.Neq },
        _                    => Array.Empty<Operator>(),
    };

    public static FactValue DefaultValue(this FactValueKind kind) => kind switch
    {
        FactValueKind.Number => new FactValue.Number(0),
        FactValueKind.Bool   => new FactValue.Bool(false),
        _                    => new FactValue.Text(""),
    };

    public static string GroupLabel(this Condition condition) => condition switch
    {
        Condition.All => "AND",
        Condition.Any => "OR",
        Condition.Not => "NOT",
        _             => "AND",
    };

    public static IReadOnlyList<Condition> GroupChildren(this Condition condition) => condition switch
    {
        Condition.All all => all.Children,
        Condition.Any any => any.Children,
        Condition.Not not => new[] { not.Inner },
        _                 => Array.Empty<Condition>(),
    };

    public static Condition WithGroupChildren(this Condition condition, IReadOnlyList<Condition> children) =>
        condition switch
        {
            Condition.All => new Condition.All(children) with { Label = condition.Label },
            Condition.Any => new Condition.Any(children) with { Label = condition.Label },
            Condition.Not => (children.Count > 0 ? new Condition.Not(children[0]) : new Condition.Not(DefaultAtom()))
                             with { Label = condition.Label },
            _             => new Condition.All(children) with { Label = condition.Label },
        };

    public static Condition DefaultGroup(string op) => op switch
    {
        "OR"  => new Condition.Any(Array.Empty<Condition>()),
        "NOT" => new Condition.Not(DefaultAtom()),
        _     => new Condition.All(Array.Empty<Condition>()),
    };

    public static Condition.Atom DefaultAtom(FieldDef? field = null) =>
        new(field?.Name ?? "", Operator.Eq, null);
}
