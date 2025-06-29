// Application/Commands/UpdateProduct.cs
using System;
using MediatR;

namespace Products.Api.Application.Commands
{
    public class UpdateProduct : IRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
    }
}
