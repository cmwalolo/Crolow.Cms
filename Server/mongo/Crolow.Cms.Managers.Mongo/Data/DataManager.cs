using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfacers.Managers;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kalow.Apps.Managers.Data
{
    public class DataManager<T> : IDataManager<T> where T : IDataObject
    {
        protected IModuleProvider databaseProvider;

        public DataManager(IModuleProvider databaseProvider)
        {
            this.databaseProvider = databaseProvider;
        }

        public T GetNode(ObjectId dataLink)
        {
            var repository = this.databaseProvider.GetContext<T>();
            var filter = Builders<T>.Filter.Eq("_id", dataLink);
            return repository.Get<T>(t => t.Id == dataLink).Result;
        }

        public T GetNode(Expression<Func<T, bool>> filter)
        {
            var nodeRepository = this.databaseProvider.GetContext<T>();
            return nodeRepository.Get<T>(filter).Result;
        }

        public IEnumerable<T> GetAllNodes()
        {
            var repository = this.databaseProvider.GetContext<T>();
            return repository.GetAll<T>().Result;
        }
        public IEnumerable<T> GetAllNodes(Expression<Func<T, bool>> filter)
        {
            var nodeRepository = this.databaseProvider.GetContext<T>();
            return nodeRepository.List<T>(filter).Result;
        }

        public void Update(T data)
        {
            var nodeRepository = this.databaseProvider.GetContext<T>();
            switch (data.EditState)
            {
                case EditState.New:
                    nodeRepository.Add<T>(data);
                    break;
                case EditState.Update:
                    nodeRepository.Update<T>(t => t.Id == data.Id, data);
                    break;
                case EditState.ToDelete:
                    nodeRepository.Remove<T>(t => t.Id == data.Id);
                    break;

            }
            data.EditState = EditState.Unchanged;
        }

        public void UpdateAll(T[] dataArray)
        {
            if (dataArray.Any())
            {
                var repository = this.databaseProvider.GetContext<T>();
                var newData = dataArray.Where(p => p.EditState == EditState.New).ToList();
                repository.AddBulk(newData);

                foreach (var data in dataArray)
                {
                    switch (data.EditState)
                    {
                        case EditState.Update:
                            repository.Update<T>(t => t.Id == data.Id, data);
                            break;
                        case EditState.ToDelete:
                            repository.Remove<T>(t => t.Id == data.Id);
                            break;

                    }
                    data.EditState = EditState.Unchanged;
                }
            }
        }
    }
}