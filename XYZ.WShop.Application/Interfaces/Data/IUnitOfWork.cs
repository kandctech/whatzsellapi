using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Interfaces.Data
{
    /// <summary>
    /// Represents a unit of work interface for managing repositories and transactions.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository for managing application users.
        /// </summary>
        IApplicationUserRepository ApplicationUsers { get; }


        /// <summary>
        /// Saves changes synchronously.
        /// </summary>
        int Complete();

        /// <summary>
        /// Saves changes asynchronously.
        /// </summary>
        Task<int> CompleteAsync();
    }
}
