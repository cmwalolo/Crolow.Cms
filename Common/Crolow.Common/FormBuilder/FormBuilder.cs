// DynamicForms.LayoutTree.cs
// Layout-first, container-tree form definition
// - One container type for all hierarchy levels
// - "Tab/Section/Panel" are just presentation kinds on the container
// - Fields have binding + labels + simple flags; no editor/validator configs here
// - Embedded forms can embed a whole FormDefinition OR just a container subtree

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DynamicForms.LayoutTree;

// ==========================================================
// 1) Form + Container tree
// ==========================================================

public sealed class FormDefinition
{
    public string SchemaVersion { get; set; } = "1.0";
    public string? Title { get; set; }

    /// <summary>
    /// Root container for the form. Treat it as a "page" or "document root".
    /// </summary>
    public ContainerNode Root { get; set; } = ContainerNode.Root();
}

public enum ContainerKind
{
    /// <summary>Generic vertical stack/group.</summary>
    Stack,

    /// <summary>Tabbed container; children usually represent tabs.</summary>
    Tabs,

    /// <summary>A tab page; usually child of Tabs.</summary>
    Tab,

    /// <summary>Collapsible group/section.</summary>
    Section,

    /// <summary>Panel/card-like group; often supports columns/grid.</summary>
    Panel
}

public sealed class ContainerNode
{
    public string Name { get; set; } = string.Empty;            // internal name/id
    public string? Title { get; set; }                          // UI title
    public string? Description { get; set; }

    /// <summary>
    /// Presentation kind (tabs/tab/section/panel/stack). Renderer decides behavior.
    /// </summary>
    public ContainerKind Kind { get; set; } = ContainerKind.Stack;

    /// <summary>Optional icon name (UI-specific interpretation).</summary>
    public string? Icon { get; set; }

    /// <summary>Section-like behavior.</summary>
    public bool Collapsible { get; set; } = false;
    public bool InitiallyExpanded { get; set; } = true;

    /// <summary>Panel-like layout options.</summary>
    public ContainerLayout Layout { get; set; } = new();

    /// <summary>Panel-like visual options.</summary>
    public ContainerVisual Visual { get; set; } = new();

    /// <summary>Child containers (subsections, tabs, panels...).</summary>
    public List<ContainerNode> Containers { get; set; } = new();

    /// <summary>Fields directly inside this container.</summary>
    public List<IFieldNode> Fields { get; set; } = new();

    public static ContainerNode Root() => new()
    {
        Name = "$root",
        Kind = ContainerKind.Stack,
        Collapsible = false
    };
}

public sealed class ContainerLayout
{
    /// <summary>
    /// Used mainly for Panel kind. Interpreted as a grid column count.
    /// </summary>
    public int Columns { get; set; } = 1;

    public int ColumnGap { get; set; } = 2;
}

public sealed class ContainerVisual
{
    public bool Outlined { get; set; } = true;
    public int Elevation { get; set; } = 0;
    public bool Dense { get; set; } = false;
    public string? CssClass { get; set; }
}

// ==========================================================
// 2) Fields (layout-first)
// ==========================================================

public enum FieldKind
{
    Value,
    EmbeddedForm,
    EmbeddedContainer
}

public enum ValueKind
{
    String,
    Number,
    Boolean,
    Date,
    Time,
    DateTime,
    Enum,
    Object,
    Collection
}

public interface IFieldNode
{
    FieldKind Kind { get; }

    string Name { get; }
    string PropertyPath { get; }     // "Address.City"
    string? Label { get; }
    string? Description { get; }
    bool Disabled { get; }
    bool ReadOnly { get; }
}

public sealed class ValueFieldNode : IFieldNode
{
    public FieldKind Kind => FieldKind.Value;

    public string Name { get; set; } = string.Empty;
    public string PropertyPath { get; set; } = string.Empty;

    public string? Label { get; set; }
    public string? Description { get; set; }
    public bool Disabled { get; set; }
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Enough for later preset resolution (choose editor/validators).
    /// </summary>
    public ValueKind ValueKind { get; set; }

    /// <summary>
    /// Structural intent. Presets can translate this into validators.
    /// </summary>
    public bool Required { get; set; }
}

public enum EmbeddedPresentation
{
    Inline,
    Panel,
    Section,
    Tab
}

public sealed class EmbeddedFormFieldNode : IFieldNode
{
    public FieldKind Kind => FieldKind.EmbeddedForm;

    public string Name { get; set; } = string.Empty;
    public string PropertyPath { get; set; } = string.Empty;

    public string? Label { get; set; }
    public string? Description { get; set; }
    public bool Disabled { get; set; }
    public bool ReadOnly { get; set; }

    public FormDefinition EmbeddedForm { get; set; } = new();
    public EmbeddedPresentation Presentation { get; set; } = EmbeddedPresentation.Inline;
}

/// <summary>
/// Embeds a container subtree (a partial form) rather than a full FormDefinition.
/// Useful when you want "address block" without forcing its own Tabs.
/// </summary>
public sealed class EmbeddedContainerFieldNode : IFieldNode
{
    public FieldKind Kind => FieldKind.EmbeddedContainer;

    public string Name { get; set; } = string.Empty;
    public string PropertyPath { get; set; } = string.Empty;

    public string? Label { get; set; }
    public string? Description { get; set; }
    public bool Disabled { get; set; }
    public bool ReadOnly { get; set; }

    public ContainerNode EmbeddedContainer { get; set; } = ContainerNode.Root();
    public EmbeddedPresentation Presentation { get; set; } = EmbeddedPresentation.Inline;
}

// ==========================================================
// 3) Builder (one container API + optional aliases)
// ==========================================================

public sealed class FormBuilder<TModel>
{
    private readonly FormDefinition _form = new();

    private FormBuilder(string? title)
    {
        _form.Title = title;
        _form.Root = ContainerNode.Root();
    }

    public static FormBuilder<TModel> Create(string? title = null) => new(title);

    public FormBuilder<TModel> Title(string title) { _form.Title = title; return this; }

    /// <summary>
    /// Build inside the root container.
    /// </summary>
    public FormBuilder<TModel> Layout(Action<ContainerBuilder<TModel>> build)
    {
        var cb = new ContainerBuilder<TModel>(_form.Root);
        build(cb);
        return this;
    }

    public FormDefinition Build() => _form;
}

public sealed class ContainerBuilder<TModel>
{
    private readonly ContainerNode _node;

    internal ContainerBuilder(ContainerNode node) => _node = node;

    // ----- Generic container creation -----

    public ContainerBuilder<TModel> Container(
        string name,
        Action<ContainerBuilder<TModel>> build,
        ContainerKind kind = ContainerKind.Stack,
        string? title = null,
        string? description = null,
        string? icon = null)
    {
        var child = new ContainerNode
        {
            Name = name,
            Kind = kind,
            Title = title ?? name,
            Description = description,
            Icon = icon
        };

        var cb = new ContainerBuilder<TModel>(child);
        build(cb);

        _node.Containers.Add(child);
        return this;
    }

    // ----- Convenience aliases (optional) -----
    // These just call Container(...) with a specific kind.
    // Use them when you want, ignore them when you don’t.

    public ContainerBuilder<TModel> Tabs(string name, Action<ContainerBuilder<TModel>> build, string? title = null)
        => Container(name, build, kind: ContainerKind.Tabs, title: title);

    public ContainerBuilder<TModel> Tab(string name, Action<ContainerBuilder<TModel>> build, string? title = null, string? icon = null)
        => Container(name, build, kind: ContainerKind.Tab, title: title, icon: icon);

    public ContainerBuilder<TModel> Section(
        string name,
        Action<ContainerBuilder<TModel>> build,
        string? title = null,
        bool collapsible = true,
        bool initiallyExpanded = true,
        string? description = null)
    {
        return Container(name, cb =>
        {
            cb._node.Collapsible = collapsible;
            cb._node.InitiallyExpanded = initiallyExpanded;
            build(cb);
        }, kind: ContainerKind.Section, title: title, description: description);
    }

    public ContainerBuilder<TModel> Panel(
        string name,
        Action<ContainerBuilder<TModel>> build,
        string? title = null,
        string? description = null)
        => Container(name, build, kind: ContainerKind.Panel, title: title, description: description);

    // ----- Container configuration -----

    public ContainerBuilder<TModel> Columns(int columns, int gap = 2)
    {
        _node.Layout.Columns = Math.Max(1, columns);
        _node.Layout.ColumnGap = Math.Max(0, gap);
        return this;
    }

    public ContainerBuilder<TModel> Visual(bool outlined = true, int elevation = 0, bool dense = false, string? cssClass = null)
    {
        _node.Visual.Outlined = outlined;
        _node.Visual.Elevation = elevation;
        _node.Visual.Dense = dense;
        _node.Visual.CssClass = cssClass;
        return this;
    }

    // ----- Fields -----

    public ContainerBuilder<TModel> Field<TValue>(
        string name,
        Expression<Func<TModel, TValue>> bind,
        Action<ValueFieldBuilder<TModel, TValue>>? build = null)
    {
        var fb = new ValueFieldBuilder<TModel, TValue>(name, PropertyPath.FromExpression(bind));
        build?.Invoke(fb);
        _node.Fields.Add(fb.Build());
        return this;
    }

    public ContainerBuilder<TModel> EmbeddedForm<TNested>(
        string name,
        Expression<Func<TModel, TNested>> bind,
        FormDefinition embeddedForm,
        Action<EmbeddedFormFieldBuilder>? build = null)
    {
        var eb = new EmbeddedFormFieldBuilder(name, PropertyPath.FromExpression(bind), embeddedForm);
        build?.Invoke(eb);
        _node.Fields.Add(eb.Build());
        return this;
    }

    public ContainerBuilder<TModel> EmbeddedContainer<TNested>(
        string name,
        Expression<Func<TModel, TNested>> bind,
        ContainerNode embeddedContainer,
        Action<EmbeddedContainerFieldBuilder>? build = null)
    {
        var eb = new EmbeddedContainerFieldBuilder(name, PropertyPath.FromExpression(bind), embeddedContainer);
        build?.Invoke(eb);
        _node.Fields.Add(eb.Build());
        return this;
    }
}

public sealed class ValueFieldBuilder<TModel, TValue>
{
    private readonly ValueFieldNode _field;

    internal ValueFieldBuilder(string name, string path)
    {
        _field = new ValueFieldNode
        {
            Name = name,
            PropertyPath = path,
            Label = name,
            ValueKind = ValueKindResolver.From(typeof(TValue))
        };
    }

    public ValueFieldBuilder<TModel, TValue> Label(string label) { _field.Label = label; return this; }
    public ValueFieldBuilder<TModel, TValue> Description(string description) { _field.Description = description; return this; }
    public ValueFieldBuilder<TModel, TValue> Disabled(bool disabled = true) { _field.Disabled = disabled; return this; }
    public ValueFieldBuilder<TModel, TValue> ReadOnly(bool readOnly = true) { _field.ReadOnly = readOnly; return this; }
    public ValueFieldBuilder<TModel, TValue> Required(bool required = true) { _field.Required = required; return this; }

    internal ValueFieldNode Build() => _field;
}

public sealed class EmbeddedFormFieldBuilder
{
    private readonly EmbeddedFormFieldNode _field;

    internal EmbeddedFormFieldBuilder(string name, string path, FormDefinition embedded)
    {
        _field = new EmbeddedFormFieldNode
        {
            Name = name,
            PropertyPath = path,
            Label = name,
            EmbeddedForm = embedded
        };
    }

    public EmbeddedFormFieldBuilder Label(string label) { _field.Label = label; return this; }
    public EmbeddedFormFieldBuilder Description(string description) { _field.Description = description; return this; }
    public EmbeddedFormFieldBuilder Disabled(bool disabled = true) { _field.Disabled = disabled; return this; }
    public EmbeddedFormFieldBuilder ReadOnly(bool readOnly = true) { _field.ReadOnly = readOnly; return this; }
    public EmbeddedFormFieldBuilder Presentation(EmbeddedPresentation p) { _field.Presentation = p; return this; }

    internal EmbeddedFormFieldNode Build() => _field;
}

public sealed class EmbeddedContainerFieldBuilder
{
    private readonly EmbeddedContainerFieldNode _field;

    internal EmbeddedContainerFieldBuilder(string name, string path, ContainerNode embedded)
    {
        _field = new EmbeddedContainerFieldNode
        {
            Name = name,
            PropertyPath = path,
            Label = name,
            EmbeddedContainer = embedded
        };
    }

    public EmbeddedContainerFieldBuilder Label(string label) { _field.Label = label; return this; }
    public EmbeddedContainerFieldBuilder Description(string description) { _field.Description = description; return this; }
    public EmbeddedContainerFieldBuilder Disabled(bool disabled = true) { _field.Disabled = disabled; return this; }
    public EmbeddedContainerFieldBuilder ReadOnly(bool readOnly = true) { _field.ReadOnly = readOnly; return this; }
    public EmbeddedContainerFieldBuilder Presentation(EmbeddedPresentation p) { _field.Presentation = p; return this; }

    internal EmbeddedContainerFieldNode Build() => _field;
}

// ==========================================================
// 4) Helpers
// ==========================================================

internal static class PropertyPath
{
    public static string FromExpression<TModel, TValue>(Expression<Func<TModel, TValue>> expr)
    {
        Expression body = expr.Body;
        if (body is UnaryExpression ue) body = ue.Operand;

        if (body is not MemberExpression me)
            throw new ArgumentException("Binding must be a property access like x => x.Name or x => x.Address.City");

        var parts = new Stack<string>();
        Expression? cur = me;

        while (cur is MemberExpression m)
        {
            parts.Push(m.Member.Name);
            cur = m.Expression;
        }

        return string.Join('.', parts);
    }
}

internal static class ValueKindResolver
{
    public static ValueKind From(Type t)
    {
        var nt = Nullable.GetUnderlyingType(t) ?? t;

        if (nt == typeof(string)) return ValueKind.String;
        if (nt == typeof(bool)) return ValueKind.Boolean;
        if (nt == typeof(DateTime)) return ValueKind.DateTime;
        if (nt.IsEnum) return ValueKind.Enum;

        if (IsNumber(nt)) return ValueKind.Number;

        return ValueKind.Object;
    }

    private static bool IsNumber(Type t) =>
        t == typeof(byte) || t == typeof(short) || t == typeof(int) || t == typeof(long) ||
        t == typeof(float) || t == typeof(double) || t == typeof(decimal) ||
        t == typeof(sbyte) || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong);
}
