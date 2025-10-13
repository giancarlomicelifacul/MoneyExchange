using Cambio.Infra.Repository;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Cambio.Service
{
    public class ExchangeRateResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("query")]
        public ExchangeQuery Query { get; set; } = new();

        [JsonPropertyName("info")]
        public ExchangeInfo Info { get; set; } = new();

        [JsonPropertyName("result")]
        public decimal Result { get; set; }
    }

    public class ExchangeQuery
    {
        [JsonPropertyName("from")]
        public string From { get; set; } = string.Empty;

        [JsonPropertyName("to")]
        public string To { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }

    public class ExchangeInfo
    {
        [JsonPropertyName("quote")]
        public decimal Quote { get; set; }
    }

    public class ExchangeResultDto
    {
        public string FromCurrency { get; set; } = string.Empty;
        public string ToCurrency { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal ConvertedAmount { get; set; }
    }

    public class ExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly ExchangeHistoryRepository _historyRepository;
        private const string ApiKey = "9b8a78468ad75aeff027b52834c6489a";
        private const string BaseUrl = "https://api.exchangerate.host";

        public ExchangeRateService(HttpClient httpClient, ExchangeHistoryRepository repository)
        {
            _httpClient = httpClient;
            _historyRepository = repository;
        }

        public async Task<ExchangeResultDto?> ConvertAsync(string fromCurrency, string toCurrency, decimal amount)
        {
            var url = $"{BaseUrl}/convert?from={fromCurrency}&to={toCurrency}&amount={amount}&access_key={ApiKey}";

            var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(url);

            if (response == null || !response.Success)
                return null;

            var result = new ExchangeResultDto
            {
                FromCurrency = fromCurrency,
                ToCurrency = toCurrency,
                Amount = amount,
                ConvertedAmount = response.Result
            };

            _historyRepository.Save(result);

            return result;
        }
    }
}
