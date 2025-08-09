using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Domain.Interfaces
{
    /// <summary>
    /// Represents an interface for entities that can be marked as deletable.
    /// </summary>
    public interface IDeletableEntity
    {
        bool IsActive { get; set; }
    }
}
