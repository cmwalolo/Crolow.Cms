using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface IRelationManager
    {
        IEnumerable<IRelationContainer> GetRelations(IDataObject dataObject);
        IEnumerable<IRelationContainer> GetRelations(ObjectId link);
        void Update(IRelationContainer node);
        void Update(List<IRelationContainer> node);
    }
}