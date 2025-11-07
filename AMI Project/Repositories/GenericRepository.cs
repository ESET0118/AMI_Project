using AMI_Project.Data; // ✅ Correct namespace for your DbContext
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMI.Api.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AMIDbContext _db;
        protected readonly DbSet<T> _set;
        public GenericRepository(AMIDbContext db)
        {
            _db = db;
            _set = db.Set<T>();
        }

        public IQueryable<T> Query() => _set.AsQueryable().AsNoTracking();

        public virtual async Task<T?> GetByIdAsync(object id) => await _set.FindAsync(id);

        public virtual async Task AddAsync(T entity)
        {
            await _set.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _set.AddRangeAsync(entities);
        }

        public virtual void Update(T entity) => _set.Update(entity);

        public virtual void Remove(T entity) => _set.Remove(entity);

        public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();

        public async Task<List<T>> ToListAsync(IQueryable<T> query) => await query.ToListAsync();

        public async Task<int> CountAsync(IQueryable<T> query) => await query.CountAsync();
    }
}
