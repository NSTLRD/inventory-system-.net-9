using MediatR;
using Products.Api.Domain.Entities;
using System.Collections.Generic;

namespace Products.Api.Application.Queries
{
    public class GetProductsByCategory : IRequest<IEnumerable<Product>>
    {
        public string Category { get; set; } = string.Empty;

        // Constructor con categoría
        public GetProductsByCategory(string category)
        {
            Category = category;
        }

        // Constructor sin parámetros para deserialización
        public GetProductsByCategory() { }
    }
}