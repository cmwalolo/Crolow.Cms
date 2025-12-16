using Crolow.Cms.Server.Core.Interfaces.Data;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Models.Databases;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Crolow.Cms.DataLayer.Mongo.Repositories
{
    public class Repository : IRepository
    {
        protected readonly IDatabaseContext context = null;
        protected readonly DataStore store = null;

        public Repository(IDatabaseContextManager contextManager, DataStore store)
        {
            this.context = contextManager.GetContext(store);
            this.store = store;
        }

        public void CreateIndex<T>(string name, string fields)
        {
            var col = context.Collection<T>(store);
            List<BsonDocument> indexes = col.Indexes.ListAsync().Result.ToListAsync().Result;
            if (!indexes.Any(i => i["name"].AsString == name))
            {
                CreateIndexOptions co = new CreateIndexOptions();
                co.Name = name;
                var e = Builders<T>.IndexKeys.Ascending(fields);
                col.Indexes.CreateOne(e, co);
            }
        }

        public async Task<IEnumerable<T>> GetAll<T>()
        {
            try
            {
                return await context.Collection<T>(store).Find(p => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw;
            }
        }

        public async Task<T> Get<T>(Expression<Func<T, bool>> filter)
        {
            try
            {
                return await context.Collection<T>(store)
                                .Find(filter)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw;
            }
        }

        public async Task<T> Get<T>(ObjectId id) where T : IDataObject
        {
            try
            {
                return await this.context.Collection<T>(store)
                    .Find(x => x.Id == id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw;
            }
        }

        public async Task<IEnumerable<T>> List<T>(Expression<Func<T, bool>> filter)
        {
            try
            {
                return await context.Collection<T>(store)
                                .Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw;
            }
        }

        public async Task AddBulk<T>(IEnumerable<T> items)
        {
            try
            {
                context.Collection<T>(store).InsertMany(items);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw;
            }
        }

        public async Task Add<T>(T item)
        {
            try
            {
                await context.Collection<T>(store).InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw;
            }
        }

        public async Task<bool> Remove<T>(Expression<Func<T, bool>> filter)
        {
            try
            {
                DeleteResult actionResult = await context.Collection<T>(store).DeleteOneAsync(filter);

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw;
            }
        }

        public async Task<bool> RemoveBulk<T>(IEnumerable<ObjectId> list)
        {
            try
            {
                var filter = Builders<T>.Filter.In("_id", list);
                DeleteResult actionResult = await context.Collection<T>(store).DeleteManyAsync(filter);

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw;
            }
        }
        public async Task<bool> Update<T>(Expression<Func<T, bool>> filter, T item)
        {
            try
            {
                ReplaceOneResult actionResult = await context.Collection<T>(store).ReplaceOneAsync(filter, item, new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw;
            }
        }

    }
}