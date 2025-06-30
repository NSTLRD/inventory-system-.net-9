using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Products.Api.Common.Models
{
    public class ExchangeRateResponse
    {
        [JsonPropertyName("result")]
        public string Result { get; set; } = string.Empty;

        [JsonPropertyName("documentation")]
        public string Documentation { get; set; } = string.Empty;

        [JsonPropertyName("terms_of_use")]
        public string TermsOfUse { get; set; } = string.Empty;

        [JsonPropertyName("time_last_update_unix")]
        public long TimeLastUpdateUnix { get; set; }

        [JsonPropertyName("time_last_update_utc")]
        public string TimeLastUpdateUtc { get; set; } = string.Empty;

        [JsonPropertyName("time_next_update_unix")]
        public long TimeNextUpdateUnix { get; set; }

        [JsonPropertyName("time_next_update_utc")]
        public string TimeNextUpdateUtc { get; set; } = string.Empty;

        [JsonPropertyName("base_code")]
        public string BaseCode { get; set; } = string.Empty;

        [JsonPropertyName("conversion_rates")]
        public Dictionary<string, decimal> ConversionRates { get; set; } = new Dictionary<string, decimal>();

        
        public bool SupportsCurrency(string currencyCode)
        {
            return ConversionRates.ContainsKey(currencyCode);
        }


        public decimal GetConversionRate(string currencyCode)
        {
            if (ConversionRates.TryGetValue(currencyCode, out decimal rate))
            {
                return rate;
            }
            
            throw new KeyNotFoundException($"Currency code {currencyCode} not found in available conversion rates");
        }
    }
}
