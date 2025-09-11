using AutoMapper;
using Microsoft.EntityFrameworkCore;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Creditor;
using XYZ.WShop.Application.Dtos.Product;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Services
{
    public class CreditorService : ICreditorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreditorService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseModel<CreditorResponse>> CreateCreditAsync(CreateCreditRequest credit)
        {
            var helper = new SubscriptionHelper();
            await helper.ValidateSubscriptionAsync(credit.BusinessId, _context);

            var creditor = _mapper.Map<CreditorRecord>(credit);
            creditor.CreatedDate = DateTime.UtcNow;
            creditor.RecordDate = credit.Date;
            creditor.Status = "unpaid"; // Default status

            await _context.CreditorRecords.AddAsync(creditor);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<CreditorResponse>(creditor);
            return ResponseModel<CreditorResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.CreatedSuccessfulMessage, "Credit record"),
                true);
        }

        public async Task<ResponseModel<PagedList<CreditorResponse>>> GetCreditsAsync(Guid businessId, int page = 1, int pageSize = 10, DateTime? date = null, string status = "unpaid", string search = "")
        {
            var query = _context.CreditorRecords
                .Where(c => c.BusinessId == businessId)
                .Include(c => c.Payments)
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
                query = query.Where(c =>
                    c.Name.Contains(search) ||
                    c.PhoneNumber.Contains(search) ||
                    c.Purpose.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var totalCredit = query.Sum(c => c.Amount);
            var totalPaid = query.Sum(c => c.Payments.Sum(p => p.Amount));
            var totalOutstanding = totalCredit - totalPaid;

            var creditors = await query
                .OrderByDescending(c => c.RecordDate)
                .ThenByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var creditorResponses = _mapper.Map<List<CreditorResponse>>(creditors);
            var pagedList = new PagedList<CreditorResponse>(creditorResponses, totalCount, page, pageSize);

            // Calculate payment summaries for each creditor
            foreach (var creditor in pagedList)
            {
                var creditorEntity = creditors.First(c => c.Id == creditor.Id);
                creditor.TotalPaid = creditorEntity.Payments.Sum(p => p.Amount);
                creditor.OutstandingAmount = creditorEntity.Amount - creditor.TotalPaid;
                creditor.Status = creditor.OutstandingAmount <= 0 ? "paid" : "unpaid";
            }

            return ResponseModel<PagedList<CreditorResponse>>.CreateResponse(
                pagedList,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true,
                metaData: new
                {
                    TotalCredit = totalCredit,
                    TotalPaid = totalPaid,
                    TotalOutstanding = totalOutstanding,
                    pagedList.HasNext,
                    TotalCount = totalCount
                });
        }

        public async Task<ResponseModel<CreditorResponse>> GetCreditByIdAsync(Guid id)
        {
            var creditor = await _context.CreditorRecords
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (creditor == null)
            {
                throw new EntityNotFoundException($"Credit record with ID {id} not found");
            }

            var result = _mapper.Map<CreditorResponse>(creditor);
            result.TotalPaid = creditor.Payments.Sum(p => p.Amount);
            result.OutstandingAmount = creditor.Amount - result.TotalPaid;
            result.Status = result.OutstandingAmount <= 0 ? "paid" : "unpaid";

            return ResponseModel<CreditorResponse>.CreateResponse(
                result,
                ApplicationContants.Messages.RetrievedSuccessfully,
                true);
        }

        public async Task<ResponseModel<CreditorResponse>> UpdateCreditAsync(UpdateCreditRequest credit)
        {
            var existingCreditor = await _context.CreditorRecords
                .FirstOrDefaultAsync(c => c.Id == credit.Id);

            if (existingCreditor == null)
            {
                throw new EntityNotFoundException($"Credit record with ID {credit.Id} not found");
            }

            _mapper.Map(credit, existingCreditor);
            existingCreditor.ModifiedDate = DateTime.UtcNow;

            _context.CreditorRecords.Update(existingCreditor);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<CreditorResponse>(existingCreditor);

            return ResponseModel<CreditorResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.UpdatedSuccessfully, "Credit record"),
                true);
        }

        public async Task<ResponseModel<CreditorResponse>> DeleteCreditAsync(Guid id)
        {
            var creditor = await _context.CreditorRecords.FirstOrDefaultAsync(c => c.Id == id);

            if (creditor == null)
            {
                throw new EntityNotFoundException($"Credit record with ID {id} not found");
            }

            _context.CreditorRecords.Remove(creditor);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<CreditorResponse>(creditor);
            return ResponseModel<CreditorResponse>.CreateResponse(
                result,
                string.Format(ApplicationContants.Messages.DeletedSuccessfully, "Credit record"),
                true);
        }

        public async Task<ResponseModel<CreditorResponse>> AddPaymentAsync(Guid creditId, CreditorPayment payment)
        {
            var creditor = await _context.CreditorRecords
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.Id == creditId);

            if (creditor == null)
            {
                throw new EntityNotFoundException($"Credit record with ID {creditId} not found");
            }

            var paymentEntity = _mapper.Map<CreditorPayment>(payment);
            paymentEntity.Date = DateTime.UtcNow;
            paymentEntity.BusinessId = creditor.BusinessId;

            creditor.Payments.Add(paymentEntity);

            // Update status based on total payments
            var totalPaid = creditor.Payments.Sum(p => p.Amount);
            creditor.Status = totalPaid >= creditor.Amount ? "paid" : "unpaid";
            creditor.ModifiedDate = DateTime.UtcNow;

            _context.CreditorRecords.Update(creditor);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<CreditorResponse>(creditor);
            result.TotalPaid = totalPaid;
            result.OutstandingAmount = creditor.Amount - totalPaid;
            result.Status = creditor.Status;

            return ResponseModel<CreditorResponse>.CreateResponse(
                result,
                "Payment recorded successfully",
                true);
        }
    }
}