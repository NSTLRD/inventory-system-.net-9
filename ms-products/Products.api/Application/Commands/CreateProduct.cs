using System;
using MediatR;

namespace Products.Api.Application.Commands
{
    public record CreateProduct(string Name, string Description, string Category, decimal Price, string Sku)
        : IRequest<Guid>;
}
