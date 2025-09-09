using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Dtos.Payment;
using XYZ.WShop.Application.Dtos.Transaction;
using XYZ.WShop.Application.Interfaces.Services;

namespace XYZ.WShop.API.Controllers
{
    //[Authorize]
    [Route("api/v{version:apiVersion}/transactions")]
    public class TransactionsController : BaseController
    {
        private readonly ILogger<TransactionsController> _logger;
        private readonly ITransactionService _transactionService;
        private readonly IPaystackService _paystackService;

        public TransactionsController(ILogger<TransactionsController> logger, ITransactionService tansactionService, IPaystackService paystackService)
        {
            _logger = logger;
            _transactionService = tansactionService;
            _paystackService = paystackService;
        }

        //[HttpPost("initialise")]
        //public async Task<IActionResult> CreateReference(TransactionDto request)
        //{
        //   var result =  await _transactionService.CreateTransaction(request);
      
        //    return Ok(result);
        //}

        [HttpPost("initialise")]
        public async Task<IActionResult> InitializePayment([FromBody] TransactionDto request)
        {
                var result = await _transactionService.CreateTransaction(request);
                var initializeRequest = new InitializeTransactionRequest
                {
                    Email = request?.Email,
                    Amount = request.Amount,
                    CallbackUrl = ApplicationContants.CallbackUrl,  // "https://yourwebsite.com/payment-callback",
                    Reference = result?.Data, 
                    Currency = request?.Currency
                };

              //  var response = await _paystackService.InitializeTransactionAsync(initializeRequest);

                    return Ok(new
                    {
                        reference = result.Data
                    });
           
        }

    [HttpPut]
        public async Task<IActionResult> UpdateTransaction(UpdateTransactionDto request)
        {

            var result = await _transactionService.UpdateTransaction(request);

            return Ok(result);
        }
    }
}
