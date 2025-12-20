// ==========================================================
// 3) Editors (mutable configs + factory defaults)
// ==========================================================

using DynamicForms.LayoutTree;
using System.Collections.Generic;

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
}

public sealed class TextAreaEditorConfig : IEditorConfig
{
    public int Lines { get; set; } = 4;
}

public sealed class EmailEditorConfig : IEditorConfig
{
}

public sealed class PasswordEditorConfig : IEditorConfig
{
    public bool RevealToggle { get; set; } = true;
}

public sealed class PhoneEditorConfig : IEditorConfig
{
    public string? Mask { get; set; }
}

public sealed class NumericEditorConfig : IEditorConfig
{
    public decimal Step { get; set; } = 1;
    public string? Format { get; set; }
}

public sealed class DateEditorConfig : IEditorConfig
{
    public string? Format { get; set; } = "yyyy-MM-dd";

}

public sealed class SelectEditorConfig<TItem> : IEditorConfig
{
    public bool Searchable { get; set; } = true;
    public string RootNodePath { get; set; } = "/";
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
}

public sealed class PropertyPathEditorConfig : IEditorConfig
{
    public bool AllowNested { get; set; } = true;
    public bool AllowCollections { get; set; } = false;
    public bool AllowNullables { get; set; } = true;

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