
using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Models.Data;

namespace Crolow.Cms.Server.Core.Models.Templates.Values
{
    [Template(Module = "Core", StorageKey = "DataTypes", NodePath = "templating/datatypes", NodeName = "o.Name")]
    public class DataType : DataObject
    {

        public string Name
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
