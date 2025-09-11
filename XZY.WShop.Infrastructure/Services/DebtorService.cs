using AutoMapper;
using Microsoft.EntityFrameworkCore;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Debtor;
using XYZ.WShop.Application.Dtos.Product;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Services
{
    public class DebtorService : IDebtorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DebtorService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseModel<DebtorResponse>> CreateDebtorAsync(CreateDebtRequest debt)
        {
            var helper = new SubscriptionHelper();
            await helper.ValidateSubscriptionAsync(debt.BusinessId, _context);

            var debtor = _mapper.Map<DebtorRecord>(debt);
            debtor.CreatedDate = DateTime.UtcNow;
            debtor.RecordDate = debt.Date;
            debtor.Status = "unpaid"; // Default status

            await _context.DebtorRecords.AddAsync(debtor);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<DebtorResponse>(debtor);
            return ResponseModel<DebtorResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.CreatedSuccessfulMessage, "Debt record"),
                true);
        }

        public async Task<ResponseModel<PagedList<DebtorResponse>>> GetDebtorAsync(Guid businessId, int page = 1, int pageSize = 10, DateTime? date = null, string status = "unpaid", string search = "")
        {
            var query = _context.DebtorRecords
                .Include(c=> c.Payments)
                .Where(d => d.BusinessId == businessId)
                .Include(d => d.Payments)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(d =>
                    d.Status.ToLower() == status.ToLower());
            }

            if (date.HasValue)
            {
                query = query.Where(d =>
                    d.RecordDate.Date == date.Value.Date.ToUniversalTime());
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(d =>
                    d.Name.Contains(search) ||
                    d.PhoneNumber.Contains(search) ||
                    d.Purpose.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var totalDebt = query.Sum(d => d.Amount);
            var totalPaid = query.Sum(d => d.Payments.Sum(p => p.Amount));
            var totalOutstanding = totalDebt - totalPaid;

            var debtors = await query
                .OrderByDescending(d => d.RecordDate)
                .ThenByDescending(d => d.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var debtorResponses = _mapper.Map<List<DebtorResponse>>(debtors);
            var pagedList = new PagedList<DebtorResponse>(debtorResponses, totalCount, page, pageSize);

            // Calculate payment summaries for each debtor
            foreach (var debtor in pagedList)
            {
                debtor.TotalPaid = debtor.Payments.Sum(p => p.Amount);
                debtor.OutstandingAmount = debtor.Amount - debtor.TotalPaid;
                debtor.Status = debtor.OutstandingAmount <= 0 ? "paid" : "unpaid";
            }

            return ResponseModel<PagedList<DebtorResponse>>.CreateResponse(
                pagedList,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true,
                metaData: new
                {
                    TotalDebt = totalDebt,
                    TotalPaid = totalPaid,
                    TotalOutstanding = totalOutstanding,
                    pagedList.HasNext,
                    TotalCount = totalCount
                });
        }

        public async Task<ResponseModel<DebtorResponse>> GetDebtorByIdAsync(Guid id)
        {
            var debtor = await _context.DebtorRecords
                .Include(d => d.Payments)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (debtor == null)
            {
                throw new EntityNotFoundException($"Debt record with ID {id} not found");
            }

            var result = _mapper.Map<DebtorResponse>(debtor);
            result.TotalPaid = debtor.Payments.Sum(p => p.Amount);
            result.OutstandingAmount = debtor.Amount - result.TotalPaid;
            result.Status = result.OutstandingAmount <= 0 ? "paid" : "unpaid";

            return ResponseModel<DebtorResponse>.CreateResponse(
                result,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true);
        }

        public async Task<ResponseModel<DebtorResponse>> UpdateDebtorAsync(UpdateDebtRequest debt)
        {
            var existingDebtor = await _context.DebtorRecords
                .FirstOrDefaultAsync(d => d.Id == debt.Id);

            if (existingDebtor == null)
            {
                throw new EntityNotFoundException($"Debt record with ID {debt.Id} not found");
            }

            _mapper.Map(debt, existingDebtor);
            existingDebtor.ModifiedDate = DateTime.UtcNow;

            _context.DebtorRecords.Update(existingDebtor);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<DebtorResponse>(existingDebtor);

            return ResponseModel<DebtorResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.UpdatedSuccessfully, "Debt record"),
                true);
        }

        public async Task<ResponseModel<DebtorResponse>> DeleteDebtorAsync(Guid id)
        {
            var debtor = await _context.DebtorRecords.FirstOrDefaultAsync(d => d.Id == id);

            if (debtor == null)
            {
                throw new EntityNotFoundException($"Debt record with ID {id} not found");
            }

            _context.DebtorRecords.Remove(debtor);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<DebtorResponse>(debtor);
            return ResponseModel<DebtorResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.DeletedSuccessfully, "Debt record"),
                true);
        }

        public async Task<ResponseModel<DebtorResponse>> AddPaymentAsync(Guid debtorId, DebtorPayment payment)
        {

            var debtor = await _context.DebtorRecords
                .Include(d => d.Payments)
                .FirstOrDefaultAsync(d => d.Id == debtorId);

            if (debtor == null)
            {
                throw new EntityNotFoundException($"Debt record with ID {debtorId} not found");
            }

            var paymentEntity = _mapper.Map<DebtorPayment>(payment);
            paymentEntity.Date = DateTime.UtcNow;
            paymentEntity.BusinessId = debtor.BusinessId;

            debtor.Payments.Add(paymentEntity);

            // Update status based on total payments
            var totalPaid = debtor.Payments.Sum(p => p.Amount);
            debtor.Status = totalPaid >= debtor.Amount ? "paid" : "unpaid";
            debtor.ModifiedDate = DateTime.UtcNow;

            _context.DebtorRecords.Update(debtor);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<DebtorResponse>(debtor);
            result.TotalPaid = totalPaid;
            result.OutstandingAmount = debtor.Amount - totalPaid;
            result.Status = debtor.Status;

            return ResponseModel<DebtorResponse>.CreateResponse(
                result,
                "Payment recorded successfully",
                true);
        }
    }
}