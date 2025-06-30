using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Products.Api.Application.Queries;
using Products.Api.Common.DTOs;
using Products.Api.Common.Interfaces;
using Products.Api.Domain.Entities;

namespace Products.Api.Application.Handlers.Queries
{
    public class GetAllProductsHandler : IRequestHandler<GetAllProducts, PaginatedResult<Product>>
    {
        private readonly IUnitOfWork _uow;

        public GetAllProductsHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PaginatedResult<Product>> Handle(GetAllProducts request, CancellationToken ct)
        {
            var allProducts = await _uow.Repository<Product>().GetAllAsync();
            
            // Filtrado en memoria
            var filteredProducts = request.Category != null
                ? allProducts.Where(p => p.Category == request.Category)
                : allProducts;

            var count = filteredProducts.Count();
            var pageSize = request.PageSize;
            var pageNumber = request.PageNumber;
            var result = filteredProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<Product>(result, count, pageNumber, pageSize);
        }
    }
}
