using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Models.Data;

namespace Crolow.Cms.Server.Core.Models.Templates.Values
{
    [Template(Module = "Core", StorageKey = "Transformers", NodePath = "templating/transformers", NodeName = "o.Name")]
    public class Transformer : DataObject
    {

        public string Name
        {
            get;
            set;
        }


        public string Method
        {
            get;
            set;
        }


        public string Type
        {
            get;
            set;
        }
    }
}
