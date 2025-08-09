using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Domain.Interfaces
{
    /// <summary>
    /// Represents the response model interface.
    /// </summary>
    public interface IResponseModel
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        bool IsSuccess { get; set; }
    }
}
