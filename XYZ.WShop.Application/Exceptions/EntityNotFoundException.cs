using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Exceptions
{
    public class EntityNotFoundException : BaseException
    {
        public EntityNotFoundException(string message, HttpStatusCode statusCode = HttpStatusCode.NotFound) : base(message, statusCode)
        {
        }
    }
}
