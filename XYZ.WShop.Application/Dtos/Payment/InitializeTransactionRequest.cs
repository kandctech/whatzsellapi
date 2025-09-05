using Newtonsoft.Json;

namespace XYZ.WShop.Application.Dtos.Payment
{
    public class InitializeTransactionRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("callback_url")]
        public string CallbackUrl { get; set; }

        // Optional: Add other properties as needed
        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; } = "NGN";
    }

    public class InitializeTransactionResponse
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public TransactionData Data { get; set; }
    }

    public class TransactionData
    {
        [JsonProperty("authorization_url")]
        public string AuthorizationUrl { get; set; }

        [JsonProperty("access_code")]
        public string AccessCode { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }
    }
}
