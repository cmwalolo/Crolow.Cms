// ==========================================================
// 8) Example: registries + embedded form + custom color editor
// ==========================================================

using DynamicForms.Typed;

namespace Crolow.Apps.Common.FormBuilder
{
    public static class ExampleSetup
    {
        public sealed class Address
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
        }

        public sealed class Customer
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string FavoriteColor { get; set; }
            public Address Address { get; set; } = new();
        }

        public static FormDefinition BuildAddressForm(EditorRegistry editors, ValidatorRegistry validators)
            => FormBuilder<Address>.Create(editors, validators, "Address")
                .Tab("Main", t =>
                    t.Section("Address", s =>
                        s.Panel(p => p.Columns(2)
                            .Field<string, TextEditorConfig>("Street", x => x.Street, f => f
                                .Label("Street")
                                .UseEditor("text", new TextEditorConfig(Placeholder: "Street..."))
                                .MaxLength(120)
                                .Required()
                            )
                            .Field<string, TextEditorConfig>("City", x => x.City, f => f
                                .Label("City")
                                .UseEditor("text")
                                .MaxLength(80)
                                .Required()
                            )
                            .Field<string, TextEditorConfig>("Country", x => x.Country, f => f
                                .Label("Country")
                                .UseEditor("text")
                                .MaxLength(80)
                                .Required()
                            )
                        )
                    )
                )
                .Build();

        public static FormDefinition BuildCustomerForm(EditorRegistry editors, ValidatorRegistry validators)
        {
            var addressForm = BuildAddressForm(editors, validators);

            return FormBuilder<Customer>.Create(editors, validators, "Customer")
                .Tab("General", t =>
                    t.Section("Identity", s =>
                        s.Panel(p => p.Columns(2)
                            .Field<string, TextEditorConfig>("Name", x => x.Name, f => f
                                .Label("Name")
                                .UseEditor("text", new TextEditorConfig(Placeholder: "Customer name"))
                                .MinLength(2)
                                .MaxLength(120)
                                .Required()
                            )
                            .Field<string, EmailEditorConfig>("Email", x => x.Email, f => f
                                .Label("Email")
                                .UseEditor("email", new EmailEditorConfig(Placeholder: "name@domain.com"))
                                .Regex(@"^\S+@\S+\.\S+$", "Invalid email")
                            )
                            .Field<string, ColorEditorConfig>("FavoriteColor", x => x.FavoriteColor, f => f
                                .Label("Favorite color")
                                .UseEditor("my:color") // uses registry default config
                            )
                        )
                    )
                )
                .Tab("Address", t =>
                    t.Section("Address", s =>
                        s.Panel(p => p.Columns(1)
                            .EmbeddedForm("Address", x => x.Address, addressForm, eb => eb
                                .Label("Address")
                                .Presentation(EmbeddedFormPresentation.Section)
                            )
                        )
                    )
                )
                .Build();
        }
    }
}