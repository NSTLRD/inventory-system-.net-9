using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Interfaces
{
    public interface IProduct
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        decimal Price { get; set; }
        int Stock { get; set; }
    }

    public interface IProductDto
    {
        int Id { get; }
        string Name { get; }
        string Description { get; }
        decimal Price { get; }
        int Stock { get; }
        string Currency { get; }
    }

    public interface ICurrencyConversionService
    {
        Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency);
    }

    public interface IPagedResponse<T>
    {
        List<T> Data { get; }
        int TotalCount { get; }
        int PageNumber { get; }
        int PageSize { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }
    
    public interface IRequest<out TResponse> { }
    
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}
