

using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Transaction;
using XYZ.WShop.Domain;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface ITransactionService
    {
      Task<ResponseModel<string>> CreateTransaction(TransactionDto transaction);
      Task<ResponseModel<Subscription>> UpdateTransaction(UpdateTransactionDto transaction);

    }
}
