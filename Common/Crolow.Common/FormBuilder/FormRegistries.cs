using DynamicForms.Typed;
using System.Collections.Generic;

public static class FormRegistries
{
    public static EditorRegistry Editors { get; private set; } = null!;
    public static ValidatorRegistry Validators { get; private set; } = null!;

    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized) return;

        Editors = new EditorRegistry()
            .Register<TextEditorConfig>("text", ValueKind.String)
            .Register<TextAreaEditorConfig>("textarea", ValueKind.String)
            .Register<EmailEditorConfig>("email", ValueKind.String)
            .Register<PasswordEditorConfig>("password", ValueKind.String)
            .Register<PhoneEditorConfig>("phone", ValueKind.String)
            .Register<NumericEditorConfig>("numeric", ValueKind.Number)
            .Register<DateEditorConfig>("date", ValueKind.Date)
            .Register("my:color", ValueKind.String, () => new ColorEditorConfig
            {
                AllowAlpha = true,
                DefaultFormat = "hex",
                Palette = new List<string> { "#E11D48", "#F59E0B", "#10B981", "#3B82F6", "#111827" }
            })

            // “missing editors” for a form designer
            .Register<SwitchEditorConfig>("switch", ValueKind.Boolean)
            .Register<CheckboxEditorConfig>("checkbox", ValueKind.Boolean)
            .Register<EnumEditorConfig>("enum", ValueKind.Enum)
            .Register<CollectionEditorConfig>("collection", ValueKind.Collection)
            .Register<ObjectEditorConfig>("object", ValueKind.Object)
            .Register<DictionaryEditorConfig>("dictionary", ValueKind.Object)
            .Register<TypeSelectorEditorConfig>("typeSelector", ValueKind.String)
            .Register<PropertyPathEditorConfig>("propertyPath", ValueKind.String)
            .Register<LayoutEditorConfig>("layout", ValueKind.Object)
            .Register<FormReferenceEditorConfig>("formRef", ValueKind.Object);

        Validators = new ValidatorRegistry()
            .Register<string?, EmptyValidatorConfig>("required")
            .Register<string?, MinLengthConfig>("minLength")
            .Register<string?, MaxLengthConfig>("maxLength")
            .Register<string?, RegexConfig>("regex")
            .Register<decimal, RangeDecimalConfig>("range");

        _initialized = true;
    }
}