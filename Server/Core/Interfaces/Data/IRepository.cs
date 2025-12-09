using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Crolow.Cms.Server.Core.Interfaces.Data
{
    public interface IRepository
    {
        void CreateIndex<T>(string name, string fields);
        Task Add<T>(T item);
        Task AddBulk<T>(IEnumerable<T> items);
        Task<T> Get<T>(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> List<T>(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> GetAll<T>();
        Task<bool> Remove<T>(Expression<Func<T, bool>> filter);
        Task<bool> Update<T>(Expression<Func<T, bool>> filter, T item);
    }
}