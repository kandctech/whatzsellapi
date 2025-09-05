using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos.Payment;
using XYZ.WShop.Application.Interfaces.Services;

namespace XZY.WShop.Infrastructure.Services
{
    public class PaystackService : IPaystackService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public PaystackService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = _httpClientFactory.CreateClient("PayStack");
            _configuration = configuration;
        }

        public async Task<InitializeTransactionResponse> InitializeTransactionAsync(InitializeTransactionRequest requestDto)
        {
            try
            {
                // Calculate Paystack fee (1.5% + ₦100)
                var amountInKobo = requestDto.Amount * 100;
                decimal fee = (0.015m * amountInKobo) + 100;
                decimal totalAmount = amountInKobo + fee;

                var request = new
                {
                    amount = totalAmount,
                    email = requestDto.Email,
                    currency = requestDto.Currency,
                    reference = requestDto.Reference,
                    callback_url = ApplicationContants.CallbackUrl,
                    metadata = new
                    {
                        custom_fields = new[]
                    {
                    new { display_name = "Fee", variable_name = "fee", value = fee }
                }
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("transaction/initialize", content);

                // Ensure success status
                response.EnsureSuccessStatusCode();

                // Read and deserialize the response
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<InitializeTransactionResponse>(responseContent);

                return result;
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP errors
                throw new Exception($"Paystack API error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Handle other errors
                throw new Exception($"Error initializing transaction: {ex.Message}", ex);
            }
        }
    }
    }
