using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using System.Collections.Generic;

namespace Crolow.Cms.Server.Core.Interfaces.Models.Data
{
    public interface IEntityContainer<T> where T : IDataObject
    {
        T DataObject { get; set; }
        INodeDefinition NodeDefinition { get; set; }
        ITracking Tracking { get; set; }
        List<IDataTranslation> Translations { get; set; }
        List<IRelationContainer> Relations { get; set; }
    }
}