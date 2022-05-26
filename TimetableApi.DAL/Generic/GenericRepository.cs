using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TimetableApi.EF;
using System.Linq.Expressions;

namespace TimetableApi.DAL.Generic
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        AppDbContext _context;
        DbSet<TEntity> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task CreateAsync(TEntity item)
        {
            _dbSet.Add(item);
           await _context.SaveChangesAsync();
        }

        public async Task CreateAsync(IEnumerable<TEntity> items)
        {
            await _context.BulkInsertAsync(items, options =>
            {
                options.IncludeGraph = false;
            });
        }

        public async Task<TEntity> FindByIdAsync(params object?[]? keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }
        public IQueryable<TEntity> FindWithInclude(Expression<Func<TEntity, bool>> predicate,
                                                  params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = Include(includeProperties);
            return query.Where(predicate);
        }

        public IQueryable<TEntity> Read()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<TEntity> Read(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate);
        }


        public async Task RemoveAsync(TEntity item)
        {
            _dbSet.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var usersToDelete = _dbSet.Where(predicate);
            await _context.BulkDeleteAsync(usersToDelete);
        }

        public async Task RemoveByIdAsync(params object?[]? keyValues)
        {
            TEntity entity = await FindByIdAsync(keyValues);
            if (entity == null)
                throw new ArgumentException();
            _dbSet.Remove(entity);
           await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity item)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        private IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;
            return includeProperties
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }
    }
}