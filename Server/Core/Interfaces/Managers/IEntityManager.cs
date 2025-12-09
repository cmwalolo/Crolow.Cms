using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using Crolow.Cms.Server.Core.Models.Actions;
using Crolow.Cms.Server.Core.Models.Data;
using System.Collections.Generic;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface IEntityManager<T> where T : IDataObject
    {
        EntityContainer<T> CreateEntity(INodeDefinition parent);
        List<IEntityContainer<T>> Children(DataRequest link);
        IEntityContainer<T> LoadEntity(DataRequest link);
        void UpdateEntity(EntityContainer<T> container);
    }
}