using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Interfaces.Data;

namespace XZY.WShop.Infrastructure.Data
{
    // <summary>
    /// Represents a unit of work for managing transactions across repositories.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IApplicationUserRepository ApplicationUsers { get; }




        public UnitOfWork(ApplicationDbContext applicationDbContext,
            IApplicationUserRepository applicationUserRepository

            )
        {
            _context = applicationDbContext;
            ApplicationUsers = applicationUserRepository;

        }

        /// <summary>
        /// Saves changes synchronously to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public int Complete()
        {
            return _context.SaveChanges();
        }

        /// <summary>
        /// Saves changes asynchronously to the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Disposes the context and suppresses finalization.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the context.
        /// </summary>
        /// <param name="disposing">A boolean value indicating whether the object is being disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
