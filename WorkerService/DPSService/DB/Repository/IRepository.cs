using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DPSService.DB.Repository
{
    /// <summary>
    /// Универсальный репозиторий
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Получает все сущности без ограничений
        /// </summary>
        Task<List<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] paths);

        /// <summary>
        /// Получает сущности по Id
        /// </summary>
        Task<TEntity> GetAsync(Guid id, params Expression<Func<TEntity, object>>[] paths);

        /// <summary>
        /// Получает сущность, удовлетворяющую условию, если сущность не найдена или их больше одной вызывется exception
        /// </summary>
        Task<TEntity> FindByAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] paths);

        /// <summary>
        /// Получает сущность, удовлетворяющую условию, или null, если по условию ничего не найдено
        /// </summary>
        Task<TEntity> FindByOrDefaultAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] paths);

        /// <summary>
        /// Возвращает все сущности, удовлетворяющие условию
        /// </summary>
        Task<List<TEntity>> FilterByAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] paths);

        /// <summary>
        /// Добавляет сущность
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// Добавляет список сущностей
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        Task AddAsync(IEnumerable<TEntity> items);

        /// <summary>
        /// Обновляет сущность
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Обновляет список сущностей
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task UpdateAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Удаляет сущность
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// Удаляет список сущностей
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task DeleteAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Удаляет все сущности, удовлетворяющие условию
        /// </summary>
        /// <param name="expression">Условие</param>
        /// <returns></returns>
        Task DeleteByAsync(Expression<Func<TEntity, bool>> expression);
    }
}
