using System;
using Products.Api.Domain.Events;
using Products.Api.Domain.ValueObjects;
using Products.Api.Domain.Interfaces;

namespace Products.Api.Domain.Entities
{
    public class Product : Entity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public decimal Price { get; private set; }
        public string SKU { get; private set; }
        public PriceHistory History { get; private set; }

        protected Product() 
        {
            History = new PriceHistory();
        }

        public Product(
            Guid id,
            string name,
            string description,
            string category,
            decimal price,
            string sku)
        {
            Id = id;
            Name = name;
            Description = description;
            Category = category;
            Price = price;
            SKU = sku;
            History = new PriceHistory();
        }

        // Método para actualizar las propiedades de forma segura
        public void Update(string name, string description, string category, decimal price, string sku)
        {
            // Si el precio ha cambiado, añadirlo al historial
            if (Price != price)
            {
                History.AddPriceChange(Price, price);
            }
            
            Name = name;
            Description = description;
            Category = category;
            Price = price;
            SKU = sku;
        }
    }
}
