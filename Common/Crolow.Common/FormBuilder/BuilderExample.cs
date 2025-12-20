// ==========================================================
// 8) Example: registries + embedded form + custom color editor
// ==========================================================

using DynamicForms.LayoutTree;

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



        public static FormDefinition BuildCustomerForm()
        {
            var addressBlock = ContainerNode.Root();
            new ContainerBuilder<Address>(addressBlock)
                .Section("address", s => s
                    .Panel("p", p => p.Columns(2)
                        .Field("Street", x => x.Street, f => f.Label("Street").Required())
                        .Field("City", x => x.City, f => f.Label("City").Required())
                    )
                );

            var form = FormBuilder<Customer>.Create("Customer")
                    .Layout(root => root
                        .Tabs("mainTabs", tabs => tabs
                            .Tab("general", tab => tab
                                .Section("identity", s => s
                                    .Panel("p1", p => p.Columns(2)
                                        .Field("Name", x => x.Name, f => f.Label("Name").Required())
                                        .Field("Email", x => x.Email, f => f.Label("Email"))
                                    )
                                )
                            )
                            .Tab("address", tab => tab
                                .Section("addr", s => s
                                    .Panel("pAddr", p => p.Columns(1)
                                    .EmbeddedContainer("Address", x => x.Address, addressBlock,
                                                eb => eb.Label("Address").Presentation(EmbeddedPresentation.Section))
                                    )
                                )
                            )
                        )
                    )
                    .Build();
            return form;

        }
    }
}