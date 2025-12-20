//// Built-in editor config types (examples)
//using DynamicForms.Typed;
//using System.Collections.Generic;

//public sealed record TextEditorConfig(
//    string? Placeholder = null,
//    bool Clearable = true,
//    bool Immediate = true
//) : IEditorConfig;

//public sealed record TextAreaEditorConfig(
//    string? Placeholder = null,
//    int Lines = 4,
//    bool Clearable = true,
//    bool Immediate = true
//) : IEditorConfig;

//public sealed record EmailEditorConfig(
//    string? Placeholder = null,
//    bool Clearable = true,
//    bool Immediate = true
//) : IEditorConfig;

//public sealed record PasswordEditorConfig(
//    string? Placeholder = null,
//    bool Clearable = true,
//    bool Immediate = true,
//    bool RevealToggle = true
//) : IEditorConfig;

//public sealed record PhoneEditorConfig(
//    string? Placeholder = null,
//    bool Clearable = true,
//    bool Immediate = true,
//    string? Mask = null
//) : IEditorConfig;

//public sealed record NumericEditorConfig(
//    decimal Step = 1,
//    bool Immediate = true,
//    string? Format = null
//) : IEditorConfig;

//public sealed record DateEditorConfig(
//    string? Format = "yyyy-MM-dd",
//    bool Clearable = true
//) : IEditorConfig;

//// Select is generic (items can be any type); for common cases use SelectItem below.
//public sealed record SelectEditorConfig<TItem>(
//    IReadOnlyList<TItem> Items,
//    bool Clearable = true,
//    bool Searchable = true
//) : IEditorConfig;

//public sealed record SelectItem(string Value, string Text);

//// Example custom editor config (typed)
//public sealed record ColorEditorConfig(
//    bool AllowAlpha = false,
//    string DefaultFormat = "hex",
//    IReadOnlyList<string>? Palette = null
//) : IEditorConfig;


//public sealed record SwitchEditorConfig(
//    string? TrueText = null,
//    string? FalseText = null
//) : IEditorConfig;

//public sealed record CheckboxEditorConfig(
//    string? Text = null
//) : IEditorConfig;

//// 2) Enum editor (choice of enum values)
//// UI-agnostic: renderer can reflect enum or use Items if provided.
//public sealed record EnumEditorConfig(
//    bool Clearable = false,
//    bool Searchable = false,
//    bool UseDisplayAttribute = true
//) : IEditorConfig;

//// 3) Collection/List editor (reorderable list of items)
//public sealed record CollectionEditorConfig(
//    bool AllowAdd = true,
//    bool AllowRemove = true,
//    bool AllowReorder = true,
//    bool ExpandItemsByDefault = false,
//    string? AddButtonText = null
//) : IEditorConfig;

//// 4) Object/Group editor (edits a composed object inline)
//// Use for: PanelLayout, PanelVisual, EditorConfig groups, etc.
//public sealed record ObjectEditorConfig(
//    bool Collapsible = false,
//    bool InitiallyExpanded = true,
//    string? Title = null,
//    string? Description = null
//) : IEditorConfig;

//// 5) Dictionary/Map editor (key/value)
//public sealed record DictionaryEditorConfig(
//    bool AllowAdd = true,
//    bool AllowRemove = true,
//    bool AllowEditKey = true,
//    bool AllowEditValue = true,
//    string? KeyLabel = "Key",
//    string? ValueLabel = "Value"
//) : IEditorConfig;

//// 6) Type selector editor
//// Used to choose: field kind, editor kind, validator kind, etc.
//public sealed record TypeSelectorEditorConfig(
//    bool Searchable = true,
//    bool Clearable = false,
//    string? Placeholder = null
//) : IEditorConfig;

//// 7) Property path editor (bind to model property)
//// Used for: Field.PropertyPath, embedded form binding, etc.
//public sealed record PropertyPathEditorConfig(
//    bool AllowNested = true,
//    bool AllowCollections = false,
//    bool AllowNullables = true,
//    string? Placeholder = null
//) : IEditorConfig;

//// 8) Layout editor (visual field placement, columns, etc.)
//public sealed record LayoutEditorConfig(
//    int MinColumns = 1,
//    int MaxColumns = 4,
//    bool AllowDragDrop = true,
//    bool ShowGrid = true
//) : IEditorConfig;

//// 9) Form reference editor (embed/select a form definition)
//// Used for: selecting an existing Address form to embed
//public sealed record FormReferenceEditorConfig(
//    bool AllowCreateNew = true,
//    bool AllowSelectExisting = true,
//    bool AllowInlineEdit = false
//) : IEditorConfig;