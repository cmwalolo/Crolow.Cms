using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using System.Collections.Generic;

namespace Crolow.Cms.Server.Core.Models.Data
{
    public class EntityContainer<T> : IEntityContainer<T> where T : IDataObject
    {
        public T DataObject { get; set; }
        public List<IDataTranslation> Translations { get; set; }
        public ITracking Tracking { get; set; }
        public INodeDefinition NodeDefinition { get; set; }
        public List<IRelationContainer> Relations { get; set; }
    }
}
