using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Crolow.Cms.Server.Core.Interfacers.Managers
{
    public interface IDataManager<T> where T : IDataObject
    {
        IEnumerable<T> GetAllNodes();
        IEnumerable<T> GetAllNodes(Expression<Func<T, bool>> filter);
        T GetNode(Expression<Func<T, bool>> filter);
        T GetNode(ObjectId dataLink);
        void Update(T data);
        void UpdateAll(T[] data);
    }
}