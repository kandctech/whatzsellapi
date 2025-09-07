using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Dtos.Creditor;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Domain;
using XYZ.WShop.Application.Dtos.Debtor;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface IDebtorService
    {
        Task<ResponseModel<PagedList<DebtorResponse>>> GetDebtorAsync(Guid businessId, int page, int pageSize, DateTime? date, string status, string search);
        Task<ResponseModel<DebtorResponse>> GetDebtorByIdAsync(Guid id);
        Task<ResponseModel<DebtorResponse>> CreateDebtorAsync(CreateDebtRequest debt);
        Task<ResponseModel<DebtorResponse>> UpdateDebtorAsync(UpdateDebtRequest debt);
        Task<ResponseModel<DebtorResponse>> DeleteDebtorAsync(Guid id);
        Task<ResponseModel<DebtorResponse>> AddPaymentAsync(Guid debtorId, DebtorPayment payment);
    }
}
