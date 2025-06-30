using System;
using System.Collections.Generic;

namespace Products.Api.Common.DTOs
{
    public class PriceHistoryDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        
        public List<PriceChangeDto> PriceChanges { get; set; }
        
        public PriceHistoryDto()
        {
            ProductName = string.Empty;
            PriceChanges = new List<PriceChangeDto>();
        }
        
        public PriceHistoryDto(Guid productId, string productName, List<PriceChangeDto> priceChanges)
        {
            ProductId = productId;
            ProductName = productName;
            PriceChanges = priceChanges;
        }
    }
    
    public class PriceChangeDto
    {
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime ChangeDate { get; set; }
        
        public decimal? OldPriceConverted { get; set; }
        public decimal? NewPriceConverted { get; set; }
        public string? Currency { get; set; }
        public decimal? ExchangeRate { get; set; }
        
        public PriceChangeDto() { }
        
        public PriceChangeDto(decimal oldPrice, decimal newPrice, DateTime changeDate)
        {
            OldPrice = oldPrice;
            NewPrice = newPrice;
            ChangeDate = changeDate;
        }
        
        public PriceChangeDto(decimal oldPrice, decimal newPrice, DateTime changeDate, 
            decimal? oldPriceConverted, decimal? newPriceConverted, string? currency, decimal? exchangeRate)
        {
            OldPrice = oldPrice;
            NewPrice = newPrice;
            ChangeDate = changeDate;
            OldPriceConverted = oldPriceConverted;
            NewPriceConverted = newPriceConverted;
            Currency = currency;
            ExchangeRate = exchangeRate;
        }
    }
}
