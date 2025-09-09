

using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Transaction;
using XYZ.WShop.Application.Dtos.User;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface ITransactionService
    {
      Task<ResponseModel<string>> CreateTransaction(TransactionDto transaction);
      Task<ResponseModel<UserProfileModel>> UpdateTransaction(UpdateTransactionDto transaction);

    }
}
