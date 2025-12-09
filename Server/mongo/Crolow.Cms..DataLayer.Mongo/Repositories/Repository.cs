using Crolow.Cms.Server.Core.Interfaces.Data;
using Kalow.Apps.DataLayer.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kalow.Apps.Repositories.Nodes
{
    public class Repository : IRepository
    {
        protected readonly MongoContext context = null;

        public Repository(MongoContext context)
        {
            this.context = context;
        }

        public void CreateIndex<T>(string name, string fields)
        {
            List<BsonDocument> indexes = context.Collection<T>().Indexes.ListAsync().Result.ToListAsync().Result;
            if (!indexes.Any(i => i["name"].AsString == name))
            {
                CreateIndexOptions co = new CreateIndexOptions();
                co.Name = name;
                var e = Builders<T>.IndexKeys.Ascending(fields);
                context.Collection<T>().Indexes.CreateOne(e, co);
            }
        }

        public async Task<IEnumerable<T>> GetAll<T>()
        {
            try
            {
                return await context.Collection<T>().Find(p => true).ToListAsync();
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
                return await context.Collection<T>()
                                .Find(filter)
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
                return await context.Collection<T>()
                                .Find(filter).ToListAsync<T>();
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
                context.Collection<T>().InsertMany(items);
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
                await context.Collection<T>().InsertOneAsync(item);
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
                DeleteResult actionResult = await context.Collection<T>().DeleteOneAsync(filter);

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
                ReplaceOneResult actionResult = await context.Collection<T>().ReplaceOneAsync(filter, item, new UpdateOptions { IsUpsert = true });
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