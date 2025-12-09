using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Models.Data;

namespace Crolow.Cms.Server.Core.Model.Templates.Values
{
    [Template(Schema = "Core", StorageKey = "Validators", NodePath = "templating/validators", NodeName = "o.Name")]
    public class Validator : DataObject
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
