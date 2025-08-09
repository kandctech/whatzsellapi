using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Domain.Interfaces
{
    /// <summary>
    /// Represents an entity that tracks audit information.
    /// </summary>
    public interface IAuditableEntity
    {
        /// <summary>
        /// Gets or sets the identifier of the user who created the entity.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last modified the entity.
        /// </summary>
        public Guid ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last modified.
        /// </summary>
        public DateTime ModifiedDate { get; set; }
    }
}
