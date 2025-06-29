using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Products.Api.Application.Queries;
using Products.Api.Common.Interfaces;
using Products.Api.Domain.Entities;

namespace Products.Api.Application.Handlers.Queries
{
    public class GetProductByIdHandler : IRequestHandler<GetProductById, Product>
    {
        private readonly IUnitOfWork _uow;

        public GetProductByIdHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Product> Handle(GetProductById request, CancellationToken ct)
        {
            return await _uow.Repository<Product>().GetByIdAsync(request.Id);
        }
    }
}
