using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using XYZ.WShop.Application.Interfaces.Data;

namespace XZY.WShop.Infrastructure.Data
{
    /// <summary>
    /// A generic repository class for handling CRUD operations on entities of type T.
    /// </summary>
    /// <remarks>
    /// Constructor for initializing the repository with a database context.
    /// </remarks>
    public abstract class GenericRepository<T>(ApplicationDbContext context) : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context = context;

        /// <summary>
        /// Asynchronously adds a new entity to the repository.
        /// </summary>
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        /// <summary>
        /// Deletes an entity from the repository.
        /// </summary>
        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        /// <summary>
        /// Asynchronously retrieves an entity by its unique identifier.
        /// </summary>
        public async Task<T> GetAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// Asynchronously retrieves entities based on the provided filter.
        /// </summary>
        /// <param name="filter">The filter expression to apply.</param>
        /// <returns>An asynchronous operation that yields a queryable collection of entities.</returns>
        public IQueryable<T> Get(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }


        public IQueryable<T> Get(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        /// <summary>
        /// Updates the state of an entity in the repository.
        /// </summary>
        public void Update(T entity)
        {

            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
