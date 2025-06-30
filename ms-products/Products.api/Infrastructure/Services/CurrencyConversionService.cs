using Products.Api.Common.Interfaces;
using Products.Api.Common.Models;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Products.Api.Infrastructure.Services
{
    public class CurrencyConversionService : ICurrencyConversionService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CurrencyConversionService> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly string _defaultCurrency;
        private readonly TimeSpan _cacheExpiration;
        private const string EXCHANGE_RATE_CACHE_KEY = "EXCHANGE_RATE_DATA";
        
        public CurrencyConversionService(
            HttpClient httpClient, 
            IMemoryCache cache, 
            IConfiguration configuration,
            ILogger<CurrencyConversionService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
            
            _apiKey = configuration["ExchangeRate:ApiKey"] ?? 
                throw new ArgumentNullException("ExchangeRate:ApiKey not configured");
                
            _baseUrl = configuration["ExchangeRate:BaseUrl"] ?? 
                "https://v6.exchangerate-api.com/v6/";
                
            _defaultCurrency = configuration["ExchangeRate:DefaultCurrency"] ?? "USD";
            
            int cacheMinutes = 60;
            if (!int.TryParse(configuration["ExchangeRate:CacheExpirationMinutes"], out cacheMinutes))
            {
                cacheMinutes = 60;
            }
            _cacheExpiration = TimeSpan.FromMinutes(cacheMinutes);
        }
        
        public async Task<decimal> ConvertFromUsdAsync(decimal amount, string targetCurrency)
        {
            if (string.IsNullOrEmpty(targetCurrency))
            {
                return amount; // Return the original amount if no currency specified
            }
            
            // If target currency is USD, no conversion needed
            if (targetCurrency.Equals(_defaultCurrency, StringComparison.OrdinalIgnoreCase))
            {
                return amount;
            }
           
            decimal rate = await GetExchangeRateAsync(targetCurrency);
            
            // Apply the conversion
            decimal convertedAmount = amount * rate;
            
            _logger.LogInformation("Converted {Amount} USD to {ConvertedAmount} {Currency}", 
                amount, convertedAmount, targetCurrency);
            
            return convertedAmount;
        }
        
        public async Task<bool> IsCurrencySupportedAsync(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
            {
                return false;
            }
            
            if (currencyCode.Equals(_defaultCurrency, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            try
            {
                var exchangeData = await GetExchangeRateDataAsync();
                return exchangeData.SupportsCurrency(currencyCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if currency {Currency} is supported", currencyCode);
                return false;
            }
        }
        
        public async Task<decimal> GetExchangeRateAsync(string targetCurrency)
        {
            if (string.IsNullOrEmpty(targetCurrency))
            {
                throw new ArgumentException("Target currency cannot be empty", nameof(targetCurrency));
            }
            
            // If target currency is USD, rate is 1
            if (targetCurrency.Equals(_defaultCurrency, StringComparison.OrdinalIgnoreCase))
            {
                return 1m;
            }

            var exchangeData = await GetExchangeRateDataAsync();
            
            return exchangeData.GetConversionRate(targetCurrency);
        }
        
        private async Task<ExchangeRateResponse> GetExchangeRateDataAsync()
        {
            // Try to get from cache first
            if (_cache.TryGetValue(EXCHANGE_RATE_CACHE_KEY, out ExchangeRateResponse? cachedResponse) && 
                cachedResponse != null)
            {
                _logger.LogDebug("Using cached exchange rate data from {LastUpdate}", 
                    cachedResponse.TimeLastUpdateUtc);
                return cachedResponse;
            }
            
            // If not in cache, fetch from API
            string url = $"{_baseUrl}{_apiKey}/latest/{_defaultCurrency}";
            
            _logger.LogInformation("Fetching exchange rates from API: {Url}", url);
            
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var exchangeData = JsonSerializer.Deserialize<ExchangeRateResponse>(content);
                
                if (exchangeData == null || exchangeData.Result != "success")
                {
                    throw new InvalidOperationException("Failed to fetch exchange rates from API");
                }
                
                // Cache the response
                _cache.Set(EXCHANGE_RATE_CACHE_KEY, exchangeData, _cacheExpiration);
                
                _logger.LogInformation("Exchange rates fetched successfully. Last update: {LastUpdate}", 
                    exchangeData.TimeLastUpdateUtc);
                
                return exchangeData;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching exchange rates from API");
                throw new Exception("Failed to fetch exchange rates. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when fetching exchange rates");
                throw new Exception("An unexpected error occurred while processing your request.", ex);
            }
        }
    }
}
