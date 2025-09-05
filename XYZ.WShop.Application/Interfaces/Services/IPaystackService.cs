using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Dtos.Payment;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface IPaystackService
    {
        Task<InitializeTransactionResponse> InitializeTransactionAsync(InitializeTransactionRequest request);
    }
}
