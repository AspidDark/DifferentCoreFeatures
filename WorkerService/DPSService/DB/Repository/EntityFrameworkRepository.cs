using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DPSService.DB.Repository
{
    public class EntityFrameworkRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly DbContext _dbContext;

        public EntityFrameworkRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Implementation of IRepository<T>

        public Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] paths)
        {
            return GetQuery(paths).ToListAsync();
        }

        public Task<TEntity> GetAsync(Guid id, params Expression<Func<TEntity, object>>[] paths)
        {
            return GetQuery(paths).SingleOrDefaultAsync(e => e.Id == id);
        }

        public Task<TEntity> FindByAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] paths)
        {
            return GetQuery(paths).SingleAsync(expression);
        }

        public Task<TEntity> FindByOrDefaultAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] paths)
        {
            return GetQuery(paths).SingleOrDefaultAsync(expression);
        }

        public Task<List<TEntity>> FilterByAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] paths)
        {
            return GetQuery(paths).Where(expression).ToListAsync();
        }

        public Task AddAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
            if (_dbContext.Database.CurrentTransaction == null)
            {
                return _dbContext.SaveChangesAsync();
            }

            return Task.CompletedTask;
        }

        public DbContext comtext(TEntity entity)
        {
            return _dbContext;
        }

        public Task AddAsync(IEnumerable<TEntity> items)
        {
            _dbContext.Set<TEntity>().AddRange(items);
            if (_dbContext.Database.CurrentTransaction == null)
            {
                return _dbContext.SaveChangesAsync();
            }

            return Task.CompletedTask;
        }

        public Task UpdateAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            if (_dbContext.Database.CurrentTransaction == null)
            {
                return _dbContext.SaveChangesAsync();
            }

            return Task.CompletedTask;
        }

        public Task UpdateAsync(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().UpdateRange(entities.ToArray());
            if (_dbContext.Database.CurrentTransaction == null)
            {
                return _dbContext.SaveChangesAsync();
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            if (_dbContext.Database.CurrentTransaction == null)
            {
                return _dbContext.SaveChangesAsync();
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
            if (_dbContext.Database.CurrentTransaction == null)
            {
                return _dbContext.SaveChangesAsync();
            }

            return Task.CompletedTask;
        }

        public Task DeleteByAsync(Expression<Func<TEntity, bool>> expression)
        {
            _dbContext.Set<TEntity>().RemoveRange(_dbContext.Set<TEntity>().Where(expression));
            if (_dbContext.Database.CurrentTransaction == null)
            {
                return _dbContext.SaveChangesAsync();
            }

            return Task.CompletedTask;
        }

        private IQueryable<TEntity> GetQuery(params Expression<Func<TEntity, object>>[] paths)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            foreach (var path in paths)
            {
                query = query.Include(path);
            }

            return query;
        }

        #endregion
    }
}
