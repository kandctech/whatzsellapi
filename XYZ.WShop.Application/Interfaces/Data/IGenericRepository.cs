using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Interfaces.Data
{
    /// <summary>
    /// Generic repository interface for CRUD operations with filtering abilities.
    /// </summary>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Get entity by id.
        /// </summary>
        Task<T> GetAsync(Guid id);

        /// <summary>
        /// Get all entities with.
        /// </summary>
        IQueryable<T> Get(Expression<Func<T, bool>> filter = null);

        /// <summary>
        /// Get all entities with includes and filtering.
        /// </summary>
        IQueryable<T> Get(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Add a new entity.
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Delete an entity.
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Update an entity.
        /// </summary>
        void Update(T entity);
    }
}
