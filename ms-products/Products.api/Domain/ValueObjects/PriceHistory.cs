using System;
using System.Collections.Generic;

namespace Products.Api.Domain.ValueObjects
{
    public class PriceHistory
    {
        public List<(decimal OldPrice, decimal NewPrice, DateTime At)> Entries { get; private set; } = new();
        
        public PriceHistory()
        {
            Entries = new List<(decimal OldPrice, decimal NewPrice, DateTime At)>();
        }
        
        public void AddPriceChange(decimal oldPrice, decimal newPrice)
        {
            Entries.Add((oldPrice, newPrice, DateTime.UtcNow));
        }
    }
}
