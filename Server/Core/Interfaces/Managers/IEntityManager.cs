using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using Crolow.Cms.Server.Core.Models.Actions;
using Crolow.Cms.Server.Core.Models.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface IEntityManager
    {
        EntityContainer<T> CreateEntity<T>(INodeDefinition parent) where T : IDataObject;
        List<IEntityContainer<T>> Children<T>(DataRequest link) where T : IDataObject;
        Task<IEntityContainer<T>> LoadEntity<T>(DataRequest link, INodeDefinition node) where T : IDataObject;

        void UpdateEntity<T>(EntityContainer<T> container) where T : IDataObject;
    }
}