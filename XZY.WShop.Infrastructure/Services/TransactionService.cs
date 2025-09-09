using AutoMapper;
using Microsoft.EntityFrameworkCore;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Transaction;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;
using XYZ.WShop.Application.Dtos.User;
using XZY.WShop.Infrastructure.Services.Helpers;
using Microsoft.Extensions.Configuration;

namespace XZY.WShop.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public TransactionService(ApplicationDbContext context, IMapper mapper, IConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
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
            await _context.SaveChangesAsync();

            return ResponseModel<string>.CreateResponse(
            reference,
            "Transaction created successfully",
            true);
        }

        public async Task<ResponseModel<UserProfileModel>> UpdateTransaction(UpdateTransactionDto updateTransaction)
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


                    subscription.EndDate = DateTime.UtcNow.AddDays(transaction.DaysPayFor + remainingDays);

                    await _context.SaveChangesAsync();
                }
               var  profileModel = new UserProfileModel();

                var business = await _context.Busineses
                .FirstOrDefaultAsync(b => b.Id == updateTransaction.BusinessId);

                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == updateTransaction.UserId.ToString());

                var jwtToken = TokenHelper.GenerateJwtToken(user, _config);
                profileModel.Token = jwtToken;
                profileModel.Email = user.Email;
                profileModel.FirstName = user.FirstName;
                profileModel.LastName = user.LastName;
                profileModel.BusinessName = business.Name;
                profileModel.Currency = business.Currency;
                profileModel.Slug = business.Slug;
                profileModel.BusinessId = business.Id;
                profileModel.BusinessAddress = business.Address;
                profileModel.PhoneNumber = user.PhoneNumber;
                profileModel.Role = user.Role;
                profileModel.BusinessCategory = business.Category;
                profileModel.BusinessDescription = business.Description;
                profileModel.Logo = business.Logo;
                profileModel.LastPaymentDate = subscription.ModifiedDate;
                profileModel.NextPaymentDate = subscription.EndDate;
                profileModel.ProfileImageUrl = user?.ProfileImageUrl;
                profileModel.Id = Guid.Parse(user.Id);

                return ResponseModel<UserProfileModel>.CreateResponse(
                profileModel,
               "Subscription updated successfully",
               true);
            };

            return ResponseModel<UserProfileModel>.CreateResponse(
              null,
             "Error updating subscription",
             true);

        }
    }
}
