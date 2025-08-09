using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Exceptions
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(message, statusCode)
        {
        }
    }
}
