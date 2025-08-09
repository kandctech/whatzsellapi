using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Domain.Interfaces;

namespace XYZ.WShop.Application.Dtos
{
    /// <summary>
    /// Represents a generic response model with data, exception, message, and success status.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    public class ResponseModel<T> : IResponseModel
    {
        /// <summary>
        /// Gets or sets the data of the response.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets the message of the response.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the response is successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        public object? MetaData { get; set; }

        /// <summary>
        /// Creates a new instance of the ResponseModel with the specified data, message, and success status.
        /// </summary>
        /// <param name="data">The data to include in the response.</param>
        /// <param name="message">The message to include in the response.</param>
        /// <param name="isSuccess">The success status of the response.</param>
        /// <returns>A new instance of ResponseModel.</returns>
        public static ResponseModel<T> CreateResponse(T? data, string? message, bool isSuccess, object metaData = null)
        {
            return new ResponseModel<T>
            {
                Data = data,
                Message = message,
                IsSuccess = isSuccess,
                MetaData = metaData
            };
        }
    }
}
