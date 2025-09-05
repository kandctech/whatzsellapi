using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Twilio.Rest;
using XYZ.WShop.Application.Dtos.Ticket;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Transaction;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;

namespace XZY.WShop.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TransactionService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ResponseModel<string>> CreateTransaction(TransactionDto transactionDto)
        {
          var reference = Guid.NewGuid().ToString();
            var transaction = new Transaction
            {
                Amount = transactionDto.Amount,
                BusinessId = transactionDto.BusinessId,
                Status = XYZ.WShop.Domain.Enums.PaymentStatus.Pending,
                Narration = "Subscription payment",
                Reference = reference,
                DaysPayFor = transactionDto.DaysPayFor
            };

            await _context.Transactions.AddAsync(transaction);

            return ResponseModel<string>.CreateResponse(
            reference,
            "Transaction created successfully",
            true);
        }

        public async Task<ResponseModel<Subscription>> UpdateTransaction(UpdateTransactionDto updateTransaction)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t=> t.Reference == updateTransaction.Reference);

             if (transaction != null && transaction.Status == XYZ.WShop.Domain.Enums.PaymentStatus.Pending)
            {
                transaction.Status =  XYZ.WShop.Domain.Enums.PaymentStatus.Paid;

                var subscription = await _context
                    .Subscriptions
                  .FirstOrDefaultAsync(s => s.BusinessId == transaction.BusinessId);

                if (subscription != null)
                {
                    var remainingDays = Math.Max(0, (subscription.EndDate - DateTime.Today).Days);


                    subscription.EndDate = subscription.EndDate.AddDays(transaction.DaysPayFor + remainingDays);

                    await _context.SaveChangesAsync();
                }

                return ResponseModel<Subscription>.CreateResponse(
                subscription,
               "Subscription updated successfully",
               true);
            };

            return ResponseModel<Subscription>.CreateResponse(
              null,
             "Error updating subscription",
             true);

        }
    }
}
