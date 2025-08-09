
using System.Net;

namespace XYZ.WShop.Application.Exceptions
{
    /// <summary>
    /// Represents a base exception class.
    /// </summary>
    public abstract class BaseException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code associated with the exception.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Initializes a new instance of the BaseException class with a specified message and status code.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="statusCode">The HTTP status code (default is InternalServerError).</param>
        public BaseException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the BaseException class with a specified error message, HTTP status code, and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="statusCode">The HTTP status code associated with the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public BaseException(string message, HttpStatusCode statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
