using System;
using MediatR;
using Products.Api.Domain.Entities;

namespace Products.Api.Application.Queries
{
    public class GetProductById : IRequest<Product>
    {
        public Guid Id { get; set; }

        public GetProductById(Guid id)
        {
            Id = id;
        }

        // Constructor sin parámetros para deserialización
        public GetProductById() { }
    }
}