using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Domain
{
    /// <summary>
    /// Represents the base entity with common properties.
    /// </summary>
    public class BaseEntity : IBaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the entity.
        /// </summary>
        public Guid Id { get; set; }
    }
}
