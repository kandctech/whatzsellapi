using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendMailAsync(string to, string subject, string html, string from = null);
    }
}
