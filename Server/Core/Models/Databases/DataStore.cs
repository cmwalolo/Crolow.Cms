using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Models.Data;

namespace Crolow.Cms.Server.Core.Models.Databases
{
    [Template(Schema = "Core", StorageKey = "DataStores", NodePath = "database/datastores", NodeName = "o.Schema + \".\" + o.TableName")]
    public class DataStore : DataObject
    {
        public string Schema { get; set; }
        public string TableName { get; set; }
        public string Database { get; set; }
    }
}
