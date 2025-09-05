using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using XYZ.WShop.Application.Dtos.Payment;
using XYZ.WShop.Application.Exceptions;
using XZY.WShop.Infrastructure.Data;

namespace XYZ.WShop.API.Controllers
{
    [Route("api/v{version:apiVersion}/web-hooks")]
    [ApiController]
    public class WebHooksController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;


        public WebHooksController(IMapper mapper, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _mapper = mapper;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = HttpContext.Request.Headers["x-paystack-signature"].FirstOrDefault() ??
                             HttpContext.Request.Headers["verif-hash"].FirstOrDefault();

            if (string.IsNullOrEmpty(signature))
                throw new BadRequestException("Invalid signature");

            // Verify webhook request
            if (HttpContext.Request.Headers.ContainsKey("x-paystack-signature"))
            {
                if (!IsValidPaystackWebhook(json, signature))
                    throw new BadRequestException("Invalid Paystack webhook");
            }
            else if (HttpContext.Request.Headers.ContainsKey("verif-hash"))
            {
                if (!IsValidFlutterwaveWebhook(signature))
                    throw new BadRequestException("Invalid Flutterwave webhook");
            }
            else
            {
                throw new BadRequestException("Unknown webhook provider");
            }

            // Process webhook event
            if (HttpContext.Request.Headers.ContainsKey("x-paystack-signature"))
            {
                var paystackEvent = JsonConvert.DeserializeObject<PaystackWebhookRequest>(json);
                await ProcessPaystackEvent(paystackEvent);
            }
            else if (HttpContext.Request.Headers.ContainsKey("verif-hash"))
            {
                var flutterwaveEvent = JsonConvert.DeserializeObject<FlutterwaveWebhookRequest>(json);
                await ProcessFlutterwaveEvent(flutterwaveEvent);
            }

            return Ok();
        }

        private bool IsValidPaystackWebhook(string json, string signature)
        {
            var useLive = bool.Parse(_configuration["PayoutAPI:Paystack:UseLive"]);
            var secret = _configuration["PayoutAPI:Paystack:TestKeys:Secretkey"];
            if (useLive)
            {
                secret = _configuration["PayoutAPI:Paystack:LiveKeys:Secretkey"];
            }
            var computedSignature = ComputeHMACSHA512(json, secret);
            return computedSignature == signature;
        }

        private bool IsValidFlutterwaveWebhook(string signature)
        {
            var useLive = bool.Parse(_configuration["PayoutAPI:Flutterwave:UseLive"]);
            var secret = _configuration["PayoutAPI:Flutterwave:TestKeys:SecreetHash"];
            if (useLive)
            {
                secret = _configuration["PayoutAPI:Flutterwave:LiveKeys:SecreetHash"];
            }

            return secret == signature;
        }

        private string ComputeHMACSHA512(string data, string secret)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private async Task ProcessPaystackEvent(PaystackWebhookRequest paystackEvent)
        {
            switch (paystackEvent.Event)
            {
                case "charge.success":
                    await HandleSuccessfulPayment(paystackEvent.Data);
                    break;
            }
        }


        private async Task ProcessFlutterwaveEvent(FlutterwaveWebhookRequest flutterwaveEvent)
        {
            switch (flutterwaveEvent.Event)
            {
                case "charge.completed":
                    await HandleSuccessfulPayment(flutterwaveEvent.Data);
                    break;
            }
        }

        private async Task HandleSuccessfulPayment(PaystackWebhookData data)
        {
          
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                
                var transaction = await context.Transactions
                    .FirstOrDefaultAsync(t=> t.Reference == data.Reference);

                if (transaction != null)
                {
                    transaction.Status = Domain.Enums.PaymentStatus.Paid;

                    var subscription = await context
                        .Subscriptions
                      .FirstOrDefaultAsync(s => s.BusinessId == transaction.BusinessId);

                    if(subscription != null)
                    {
                        var remainingDays = Math.Max(0, (subscription.EndDate - DateTime.Today).Days);

                        subscription.EndDate =  subscription.EndDate.AddDays(transaction.DaysPayFor + remainingDays);
                        subscription.ModifiedDate = DateTime.UtcNow;

                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task HandleSuccessfulPayment(FlutterwaveWebhookData data)
        {

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                var transaction = await context.Transactions
                    .FirstOrDefaultAsync(t => t.Reference == data.tx_ref);

                if (transaction != null)
                {
                    transaction.Status = Domain.Enums.PaymentStatus.Paid;

                    var subscription = await context
                        .Subscriptions
                      .FirstOrDefaultAsync(s => s.BusinessId == transaction.BusinessId);

                    if (subscription != null)
                    {
                        var remainingDays = Math.Max(0, (subscription.EndDate - DateTime.Today).Days);

                        int daysPaidFor = (int)data.amount / ApplicationDbContext.AmountPerDay;

                        subscription.EndDate = subscription.EndDate.AddDays(daysPaidFor + remainingDays);

                        await context.SaveChangesAsync();
                    }
                }
            }
        }

    }
}
