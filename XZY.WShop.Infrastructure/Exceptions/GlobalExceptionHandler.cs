using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XYZ.WShop.Application.Exceptions;

namespace XZY.WShop.Infrastructure.Exceptions
{
    /// <summary>
    /// Represents a global exception handler that implements the IExceptionHandler interface.
    /// </summary>
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment webHostEnvironment) : IExceptionHandler
    {
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        /// <summary>
        /// Tries to handle exceptions asynchronously.
        /// </summary>
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Instance = httpContext.Request.Path
            };

            if (exception is BaseException e)
            {
                httpContext.Response.StatusCode = (int)e.StatusCode;
                problemDetails.Title = e.Message;
            }
            else
            {
                problemDetails.Title = exception.Message;
            }

            if (!_webHostEnvironment.IsDevelopment())
            {
                SentrySdk.CaptureException(exception);
            }

            logger.LogError("Unhandled Exception: {@exception}", exception);

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);

            return true;
        }
    }
}
