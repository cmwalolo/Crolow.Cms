using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Kalow.Apps.Models.Transactions
{
    public class DataContainer
    {
        ObjectId DataLink { get; set; }
        INodeDefinition NodeDefinition { get; set; }
        IDataObject Data { get; set; }
        IList<IDataTranslation> Translations { get; set; }
        ITracking Tracking { get; set; }
        IRelationContainer Relations { get; set; }
    }

}
