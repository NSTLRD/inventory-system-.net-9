using MediatR;
using Products.Api.Common.DTOs;
using Products.Api.Domain.Entities;

namespace Products.Api.Application.Queries
{
    
    public class GetAllProducts : IRequest<PaginatedResult<Product>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Category { get; set; }

        public GetAllProducts()
        {
        }

        public GetAllProducts(int pageNumber, int pageSize, string? category = null)
        {
            PageNumber = pageNumber > 0 ? pageNumber : 1;
            PageSize = pageSize > 0 ? pageSize : 10;
            Category = category;
        }
    }
}