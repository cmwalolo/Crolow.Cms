// DynamicForms.Typed.FactoryMutable.cs
// Strongly typed form definition + builder
// - NO dynamic objects for editor/validator configs
// - Config objects are MUTABLE (setters) for easy designer binding
// - Registries store FACTORIES so defaults are never shared by reference
// - Embedded forms supported

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicForms.Typed;

// ==========================================================
// 0) Shared concepts
// ==========================================================

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

public enum ValueUpdateMode
{
    Immediate,
    OnCommit
}

// ==========================================================
// 1) Core containers (serializable objects)
// ==========================================================

public sealed class FormDefinition
{
    public string SchemaVersion { get; set; } = "1.0";
    public string? Title { get; set; }
    public List<TabDefinition> Tabs { get; set; } = new();
}

public sealed class TabDefinition
{
    public string Title { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public List<SectionDefinition> Sections { get; set; } = new();
}

public sealed class SectionDefinition
{
    public string Title { get; set; } = string.Empty;
    public bool Collapsible { get; set; } = true;
    public bool InitiallyExpanded { get; set; } = true;
    public string? Description { get; set; }
    public List<PanelDefinition> Panels { get; set; } = new();
}

public sealed class PanelDefinition
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public PanelLayout Layout { get; set; } = new();
    public PanelVisual Visual { get; set; } = new();
    public List<IFieldDefinition> Fields { get; set; } = new();
}

public sealed class PanelLayout
{
    public int Columns { get; set; } = 1;
    public int ColumnGap { get; set; } = 2;
}

public sealed class PanelVisual
{
    public bool Outlined { get; set; } = true;
    public int Elevation { get; set; } = 0;
    public bool Dense { get; set; } = false;
    public string? CssClass { get; set; }
}

// ==========================================================
// 2) Fields
// ==========================================================

public interface IFieldDefinition
{
    string Name { get; set; }
    string PropertyPath { get; set; }
    string? Label { get; set; }
    string? Description { get; set; }
    bool Disabled { get; set; }
    bool ReadOnly { get; set; }
}

public enum EmbeddedFormPresentation
{
    Inline,
    Panel,
    Section,
    Tab
}

public sealed class EmbeddedFormFieldDefinition : IFieldDefinition
{
    public string Name { get; set; } = string.Empty;
    public string PropertyPath { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string? Description { get; set; }
    public bool Disabled { get; set; }
    public bool ReadOnly { get; set; }

    public FormDefinition EmbeddedForm { get; set; } = new();
    public EmbeddedFormPresentation Presentation { get; set; } = EmbeddedFormPresentation.Inline;
}

public sealed class FieldDefinition<TValue, TEditorConfig> : IFieldDefinition
    where TEditorConfig : class, IEditorConfig
{
    public string Name { get; set; } = string.Empty;
    public string PropertyPath { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string? Description { get; set; }
    public bool Disabled { get; set; }
    public bool ReadOnly { get; set; }

    public bool Required { get; set; }

    public EditorDefinition<TEditorConfig> Editor { get; set; } = new();
    public List<IValidatorDefinition<TValue>> Validators { get; set; } = new();
}

// ==========================================================
// 3) Editors (mutable configs + factory defaults)
// ==========================================================

public interface IEditorConfig { }

public sealed class EditorDefinition<TConfig>
    where TConfig : class, IEditorConfig
{
    public ValueKind ValueKind { get; set; }
    public string Key { get; set; } = string.Empty;
    public TConfig Config { get; set; } = default!;
}

// Built-in editor config types (mutable, readable)

public sealed class TextEditorConfig : IEditorConfig
{
    public string? Placeholder { get; set; }
    public bool AllowClear { get; set; } = true;
    public ValueUpdateMode UpdateMode { get; set; } = ValueUpdateMode.Immediate;
}

public sealed class TextAreaEditorConfig : IEditorConfig
{
    public string? Placeholder { get; set; }
    public int Lines { get; set; } = 4;
    public bool AllowClear { get; set; } = true;
    public ValueUpdateMode UpdateMode { get; set; } = ValueUpdateMode.Immediate;
}

public sealed class EmailEditorConfig : IEditorConfig
{
    public string? Placeholder { get; set; }
    public bool AllowClear { get; set; } = true;
    public ValueUpdateMode UpdateMode { get; set; } = ValueUpdateMode.Immediate;
}

public sealed class PasswordEditorConfig : IEditorConfig
{
    public string? Placeholder { get; set; }
    public bool AllowClear { get; set; } = true;
    public ValueUpdateMode UpdateMode { get; set; } = ValueUpdateMode.Immediate;
    public bool RevealToggle { get; set; } = true;
}

public sealed class PhoneEditorConfig : IEditorConfig
{
    public string? Placeholder { get; set; }
    public bool AllowClear { get; set; } = true;
    public ValueUpdateMode UpdateMode { get; set; } = ValueUpdateMode.Immediate;
    public string? Mask { get; set; }
}

public sealed class NumericEditorConfig : IEditorConfig
{
    public decimal Step { get; set; } = 1;
    public ValueUpdateMode UpdateMode { get; set; } = ValueUpdateMode.Immediate;
    public string? Format { get; set; }
}

public sealed class DateEditorConfig : IEditorConfig
{
    public string? Format { get; set; } = "yyyy-MM-dd";
    public bool AllowClear { get; set; } = true;
}

public sealed class SelectEditorConfig<TItem> : IEditorConfig
{
    public List<TItem> Items { get; set; } = new();
    public bool AllowClear { get; set; } = true;
    public bool Searchable { get; set; } = true;
}

public sealed class SelectItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public sealed class ColorEditorConfig : IEditorConfig
{
    public bool AllowAlpha { get; set; } = false;
    public string DefaultFormat { get; set; } = "hex";
    public List<string>? Palette { get; set; }
}

// “Form designer” meta editors
public sealed class SwitchEditorConfig : IEditorConfig
{
    public string? TrueText { get; set; }
    public string? FalseText { get; set; }
}

public sealed class CheckboxEditorConfig : IEditorConfig
{
    public string? Text { get; set; }
}

public sealed class EnumEditorConfig : IEditorConfig
{
    public bool AllowClear { get; set; } = false;
    public bool Searchable { get; set; } = false;
    public bool UseDisplayAttribute { get; set; } = true;
}

public sealed class CollectionEditorConfig : IEditorConfig
{
    public bool AllowAdd { get; set; } = true;
    public bool AllowRemove { get; set; } = true;
    public bool AllowReorder { get; set; } = true;
    public bool ExpandItemsByDefault { get; set; } = false;
    public string? AddButtonText { get; set; }
}

public sealed class ObjectEditorConfig : IEditorConfig
{
    public bool Collapsible { get; set; } = false;
    public bool InitiallyExpanded { get; set; } = true;
    public string? Title { get; set; }
    public string? Description { get; set; }
}

public sealed class DictionaryEditorConfig : IEditorConfig
{
    public bool AllowAdd { get; set; } = true;
    public bool AllowRemove { get; set; } = true;
    public bool AllowEditKey { get; set; } = true;
    public bool AllowEditValue { get; set; } = true;
    public string KeyLabel { get; set; } = "Key";
    public string ValueLabel { get; set; } = "Value";
}

public sealed class TypeSelectorEditorConfig : IEditorConfig
{
    public bool Searchable { get; set; } = true;
    public bool AllowClear { get; set; } = false;
    public string? Placeholder { get; set; }
}

public sealed class PropertyPathEditorConfig : IEditorConfig
{
    public bool AllowNested { get; set; } = true;
    public bool AllowCollections { get; set; } = false;
    public bool AllowNullables { get; set; } = true;
    public string? Placeholder { get; set; }
}

public sealed class LayoutEditorConfig : IEditorConfig
{
    public int MinColumns { get; set; } = 1;
    public int MaxColumns { get; set; } = 4;
    public bool AllowDragDrop { get; set; } = true;
    public bool ShowGrid { get; set; } = true;
}

public sealed class FormReferenceEditorConfig : IEditorConfig
{
    public bool AllowCreateNew { get; set; } = true;
    public bool AllowSelectExisting { get; set; } = true;
    public bool AllowInlineEdit { get; set; } = false;
}

// ==========================================================
// 4) Validators (mutable configs + factory defaults)
// ==========================================================

public interface IValidatorConfig { }

public interface IValidatorDefinition<TValue>
{
    string Key { get; }
    string Message { get; }
}

public interface IValidatorDefinition<TValue, out TConfig> : IValidatorDefinition<TValue>
    where TConfig : class, IValidatorConfig
{
    TConfig Config { get; }
}

public sealed class ValidatorDefinition<TValue, TConfig> : IValidatorDefinition<TValue, TConfig>
    where TConfig : class, IValidatorConfig
{
    public ValidatorDefinition() { }

    public ValidatorDefinition(string key, string message, TConfig config)
    {
        Key = key;
        Message = message;
        Config = config;
    }

    public string Key { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public TConfig Config { get; set; } = default!;
}

// Built-in validator configs (mutable)

public sealed class EmptyValidatorConfig : IValidatorConfig { }

public sealed class MinLengthConfig : IValidatorConfig
{
    public int Min { get; set; }
}

public sealed class MaxLengthConfig : IValidatorConfig
{
    public int Max { get; set; }
}

public sealed class RegexConfig : IValidatorConfig
{
    public string Pattern { get; set; } = string.Empty;
}

public sealed class RangeDecimalConfig : IValidatorConfig
{
    public decimal Min { get; set; }
    public decimal Max { get; set; }
}

// ==========================================================
// 5) Registries (FACTORY defaults, no shared instances)
// ==========================================================

public sealed class EditorDescriptor<TConfig>
    where TConfig : class, IEditorConfig
{
    public EditorDescriptor(string key, ValueKind valueKind, Func<TConfig> createDefault)
    {
        Key = key;
        ValueKind = valueKind;
        CreateDefault = createDefault;
    }

    public string Key { get; }
    public ValueKind ValueKind { get; }
    public Func<TConfig> CreateDefault { get; }
}

public interface IEditorRegistry
{
    EditorDescriptor<TConfig> Get<TConfig>(string key) where TConfig : class, IEditorConfig;
}

public sealed class EditorRegistry : IEditorRegistry
{
    private readonly Dictionary<string, object> _map = new(StringComparer.OrdinalIgnoreCase);

    public EditorRegistry Register<TConfig>(string key, ValueKind valueKind, Func<TConfig> factory)
        where TConfig : class, IEditorConfig
    {
        _map[key] = new EditorDescriptor<TConfig>(key, valueKind, factory);
        return this;
    }

    public EditorRegistry Register<TConfig>(string key, ValueKind valueKind)
        where TConfig : class, IEditorConfig, new()
        => Register(key, valueKind, () => new TConfig());

    public EditorDescriptor<TConfig> Get<TConfig>(string key)
        where TConfig : class, IEditorConfig
    {
        if (!_map.TryGetValue(key, out var obj))
            throw new InvalidOperationException($"Editor '{key}' is not registered.");

        if (obj is not EditorDescriptor<TConfig> typed)
            throw new InvalidOperationException($"Editor '{key}' is registered but with a different config type. Expected {typeof(TConfig).Name}.");

        return typed;
    }
}

public sealed class ValidatorDescriptor<TValue, TConfig>
    where TConfig : class, IValidatorConfig
{
    public ValidatorDescriptor(string key, Func<TConfig> createDefault)
    {
        Key = key;
        CreateDefault = createDefault;
    }

    public string Key { get; }
    public Func<TConfig> CreateDefault { get; }
}

public interface IValidatorRegistry
{
    ValidatorDescriptor<TValue, TConfig> Get<TValue, TConfig>(string key)
        where TConfig : class, IValidatorConfig;
}

public sealed class ValidatorRegistry : IValidatorRegistry
{
    private readonly Dictionary<string, object> _map = new(StringComparer.OrdinalIgnoreCase);

    public ValidatorRegistry Register<TValue, TConfig>(string key, Func<TConfig> factory)
        where TConfig : class, IValidatorConfig
    {
        _map[key] = new ValidatorDescriptor<TValue, TConfig>(key, factory);
        return this;
    }

    public ValidatorRegistry Register<TValue, TConfig>(string key)
        where TConfig : class, IValidatorConfig, new()
        => Register<TValue, TConfig>(key, () => new TConfig());

    public ValidatorDescriptor<TValue, TConfig> Get<TValue, TConfig>(string key)
        where TConfig : class, IValidatorConfig
    {
        if (!_map.TryGetValue(key, out var obj))
            throw new InvalidOperationException($"Validator '{key}' is not registered.");

        if (obj is not ValidatorDescriptor<TValue, TConfig> typed)
            throw new InvalidOperationException($"Validator '{key}' is registered but with a different value/config type.");

        return typed;
    }
}

// ==========================================================
// 6) Fluent Builder (uses registries; defaults are fresh instances)
// ==========================================================

public sealed class FormBuilder<TModel>
{
    private readonly IEditorRegistry _editors;
    private readonly IValidatorRegistry _validators;
    private readonly FormDefinition _form = new();

    private FormBuilder(IEditorRegistry editors, IValidatorRegistry validators, string? title)
    {
        _editors = editors;
        _validators = validators;
        _form.Title = title;
    }

    public static FormBuilder<TModel> Create(IEditorRegistry editors, IValidatorRegistry validators, string? title = null)
        => new(editors, validators, title);

    public FormBuilder<TModel> Title(string title) { _form.Title = title; return this; }

    public FormBuilder<TModel> Tab(string title, Action<TabBuilder<TModel>> build, string? icon = null)
    {
        var tb = new TabBuilder<TModel>(_editors, _validators, title, icon);
        build(tb);
        _form.Tabs.Add(tb.Build());
        return this;
    }

    public FormDefinition Build() => _form;
}

public sealed class TabBuilder<TModel>
{
    private readonly IEditorRegistry _editors;
    private readonly IValidatorRegistry _validators;
    private readonly TabDefinition _tab;

    internal TabBuilder(IEditorRegistry editors, IValidatorRegistry validators, string title, string? icon)
    {
        _editors = editors;
        _validators = validators;
        _tab = new TabDefinition { Title = title, Icon = icon };
    }

    public TabBuilder<TModel> Section(
        string title,
        Action<SectionBuilder<TModel>> build,
        bool collapsible = true,
        bool initiallyExpanded = true,
        string? description = null)
    {
        var sb = new SectionBuilder<TModel>(_editors, _validators, title, collapsible, initiallyExpanded, description);
        build(sb);
        _tab.Sections.Add(sb.Build());
        return this;
    }

    internal TabDefinition Build() => _tab;
}

public sealed class SectionBuilder<TModel>
{
    private readonly IEditorRegistry _editors;
    private readonly IValidatorRegistry _validators;
    private readonly SectionDefinition _section;

    internal SectionBuilder(IEditorRegistry editors, IValidatorRegistry validators, string title, bool collapsible, bool expanded, string? description)
    {
        _editors = editors;
        _validators = validators;
        _section = new SectionDefinition
        {
            Title = title,
            Collapsible = collapsible,
            InitiallyExpanded = expanded,
            Description = description
        };
    }

    public SectionBuilder<TModel> Panel(Action<PanelBuilder<TModel>> build, string? title = null, string? description = null)
    {
        var pb = new PanelBuilder<TModel>(_editors, _validators, title, description);
        build(pb);
        _section.Panels.Add(pb.Build());
        return this;
    }

    internal SectionDefinition Build() => _section;
}

public sealed class PanelBuilder<TModel>
{
    private readonly IEditorRegistry _editors;
    private readonly IValidatorRegistry _validators;
    private readonly PanelDefinition _panel = new();

    internal PanelBuilder(IEditorRegistry editors, IValidatorRegistry validators, string? title, string? description)
    {
        _editors = editors;
        _validators = validators;
        _panel.Title = title;
        _panel.Description = description;
    }

    public PanelBuilder<TModel> Columns(int columns, int gap = 2)
    {
        _panel.Layout.Columns = Math.Max(1, columns);
        _panel.Layout.ColumnGap = Math.Max(0, gap);
        return this;
    }

    public PanelBuilder<TModel> Visual(bool outlined = true, int elevation = 0, bool dense = false, string? cssClass = null)
    {
        _panel.Visual.Outlined = outlined;
        _panel.Visual.Elevation = elevation;
        _panel.Visual.Dense = dense;
        _panel.Visual.CssClass = cssClass;
        return this;
    }

    public PanelBuilder<TModel> Field<TValue, TEditorConfig>(
        string name,
        Expression<Func<TModel, TValue>> bind,
        Action<FieldBuilder<TModel, TValue, TEditorConfig>> build)
        where TEditorConfig : class, IEditorConfig
    {
        var fb = new FieldBuilder<TModel, TValue, TEditorConfig>(_editors, _validators, name, bind);
        build(fb);
        _panel.Fields.Add(fb.Build());
        return this;
    }

    public PanelBuilder<TModel> EmbeddedForm<TNested>(
        string name,
        Expression<Func<TModel, TNested>> bind,
        FormDefinition embeddedForm,
        Action<EmbeddedFormFieldBuilder>? build = null)
    {
        var eb = new EmbeddedFormFieldBuilder(name, PropertyPath.FromExpression(bind), embeddedForm);
        build?.Invoke(eb);
        _panel.Fields.Add(eb.Build());
        return this;
    }

    internal PanelDefinition Build() => _panel;
}

public sealed class FieldBuilder<TModel, TValue, TEditorConfig>
    where TEditorConfig : class, IEditorConfig
{
    private readonly IEditorRegistry _editors;
    private readonly IValidatorRegistry _validators;
    private readonly string _name;
    private readonly Expression<Func<TModel, TValue>> _expr;

    private string? _label;
    private string? _description;
    private bool _disabled;
    private bool _readOnly;
    private bool _required;

    private EditorDefinition<TEditorConfig>? _editor;
    private readonly List<IValidatorDefinition<TValue>> _validatorDefs = new();

    internal FieldBuilder(IEditorRegistry editors, IValidatorRegistry validators, string name, Expression<Func<TModel, TValue>> expr)
    {
        _editors = editors;
        _validators = validators;
        _name = name;
        _expr = expr;
    }

    public FieldBuilder<TModel, TValue, TEditorConfig> Label(string label) { _label = label; return this; }
    public FieldBuilder<TModel, TValue, TEditorConfig> Description(string description) { _description = description; return this; }
    public FieldBuilder<TModel, TValue, TEditorConfig> Disabled(bool disabled = true) { _disabled = disabled; return this; }
    public FieldBuilder<TModel, TValue, TEditorConfig> ReadOnly(bool readOnly = true) { _readOnly = readOnly; return this; }

    public FieldBuilder<TModel, TValue, TEditorConfig> Required(bool required = true, string message = "Required")
    {
        _required = required;

        if (required && _validatorDefs.All(v => v.Key != "required"))
        {
            _validatorDefs.Insert(0, new ValidatorDefinition<TValue, EmptyValidatorConfig>(
                "required",
                message,
                new EmptyValidatorConfig()
            ));
        }

        return this;
    }

    // Editor selection — default config is a fresh instance from the factory
    public FieldBuilder<TModel, TValue, TEditorConfig> UseEditor(string key, TEditorConfig? config = null)
    {
        var d = _editors.Get<TEditorConfig>(key);

        _editor = new EditorDefinition<TEditorConfig>
        {
            ValueKind = d.ValueKind,
            Key = d.Key,
            Config = config ?? d.CreateDefault()
        };

        return this;
    }

    // Typed validator selection — default config is a fresh instance from the factory
    public FieldBuilder<TModel, TValue, TEditorConfig> UseValidator<TValConfig>(
        string key,
        string message,
        TValConfig? config = null)
        where TValConfig : class, IValidatorConfig
    {
        var d = _validators.Get<TValue, TValConfig>(key);

        _validatorDefs.Add(new ValidatorDefinition<TValue, TValConfig>(
            d.Key,
            message,
            config ?? d.CreateDefault()
        ));

        return this;
    }

    public FieldBuilder<TModel, TValue, TEditorConfig> MinLength(int min, string? message = null)
        => UseValidator("minLength", message ?? $"Minimum length is {min}", new MinLengthConfig { Min = min });

    public FieldBuilder<TModel, TValue, TEditorConfig> MaxLength(int max, string? message = null)
        => UseValidator("maxLength", message ?? $"Maximum length is {max}", new MaxLengthConfig { Max = max });

    public FieldBuilder<TModel, TValue, TEditorConfig> Regex(string pattern, string? message = null)
        => UseValidator("regex", message ?? "Invalid format", new RegexConfig { Pattern = pattern });

    public FieldBuilder<TModel, TValue, TEditorConfig> Range(decimal min, decimal max, string? message = null)
        => UseValidator("range", message ?? $"Must be between {min} and {max}",
            new RangeDecimalConfig { Min = min, Max = max });

    internal FieldDefinition<TValue, TEditorConfig> Build()
    {
        if (_editor is null)
            throw new InvalidOperationException($"Field '{_name}' has no editor. Call UseEditor(key, config).");

        var field = new FieldDefinition<TValue, TEditorConfig>
        {
            Name = _name,
            PropertyPath = PropertyPath.FromExpression(_expr),
            Label = _label ?? _name,
            Description = _description,
            Disabled = _disabled,
            ReadOnly = _readOnly,
            Required = _required,
            Editor = _editor,
            Validators = _validatorDefs.ToList()
        };

        // Ensure required validator presence if Required==true
        if (field.Required && field.Validators.All(v => v.Key != "required"))
        {
            field.Validators.Insert(0, new ValidatorDefinition<TValue, EmptyValidatorConfig>(
                "required",
                "Required",
                new EmptyValidatorConfig()
            ));
        }

        return field;
    }
}

public sealed class EmbeddedFormFieldBuilder
{
    private readonly EmbeddedFormFieldDefinition _field;

    internal EmbeddedFormFieldBuilder(string name, string path, FormDefinition embedded)
    {
        _field = new EmbeddedFormFieldDefinition
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
    public EmbeddedFormFieldBuilder Presentation(EmbeddedFormPresentation presentation) { _field.Presentation = presentation; return this; }

    internal EmbeddedFormFieldDefinition Build() => _field;
}

// ==========================================================
// 7) PropertyPath helper
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
