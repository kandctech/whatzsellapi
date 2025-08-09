using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Domain.Enums
{
    /// <summary>
    /// Represents the type of audit actions.
    /// </summary>
    public enum AuditType
    {
        /// <summary>
        /// No action.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a create action.
        /// </summary>
        Create = 1,

        /// <summary>
        /// Represents an update action.
        /// </summary>
        Update = 2,

        /// <summary>
        /// Represents a delete action.
        /// </summary>
        Delete = 3
    }
}
