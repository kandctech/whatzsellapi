using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Creditor;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Domain;

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface ICreditorService
    {
        Task<ResponseModel<PagedList<CreditorResponse>>> GetCreditsAsync(Guid businessId, int page, int pageSize, DateTime? date, string status, string search);
        Task<ResponseModel<CreditorResponse>> GetCreditByIdAsync(Guid id);
        Task<ResponseModel<CreditorResponse>> CreateCreditAsync(CreateCreditRequest credit);
        Task<ResponseModel<CreditorResponse>> UpdateCreditAsync(UpdateCreditRequest credit);
        Task<ResponseModel<CreditorResponse>> DeleteCreditAsync(Guid id);
        Task<ResponseModel<CreditorResponse>> AddPaymentAsync(Guid creditId, CreditorPayment payment);
    }
}
