using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AMI.Api.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> Query();
        Task<T?> GetByIdAsync(object id);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        Task<int> SaveChangesAsync();
        Task<List<T>> ToListAsync(IQueryable<T> query);
        Task<int> CountAsync(IQueryable<T> query);
    }
}
