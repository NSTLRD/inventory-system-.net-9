using System.Threading.Tasks;

namespace Products.Api.Common.Interfaces
{
    public interface ICurrencyConversionService
    {
        /// <summary>
        /// Converts an amount from USD to the specified target currency
        /// </summary>
        /// <param name="amount">The amount in USD to convert</param>
        /// <param name="targetCurrency">The target currency code (e.g., "DOP")</param>
        /// <returns>The converted amount in the target currency</returns>
        Task<decimal> ConvertFromUsdAsync(decimal amount, string targetCurrency);

        /// <summary>
        /// Checks if a given currency code is supported
        /// </summary>
        /// <param name="currencyCode">The currency code to check</param>
        /// <returns>True if the currency is supported, false otherwise</returns>
        Task<bool> IsCurrencySupportedAsync(string currencyCode);

        /// <summary>
        /// Gets the current exchange rate from USD to the specified target currency
        /// </summary>
        /// <param name="targetCurrency">The target currency code (e.g., "DOP")</param>
        /// <returns>The exchange rate from USD to the target currency</returns>
        Task<decimal> GetExchangeRateAsync(string targetCurrency);
    }
}
