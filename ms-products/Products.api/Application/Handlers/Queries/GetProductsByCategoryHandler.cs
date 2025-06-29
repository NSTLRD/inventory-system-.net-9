using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Products.Api.Application.Queries;
using Products.Api.Common.DTOs;
using Products.Api.Common.Interfaces;
using Products.Api.Domain.Entities;


namespace Products.Api.Application.Handlers.Queries
{
    public class GetProductsByCategoryHandler : IRequestHandler<GetProductsByCategory, IEnumerable<Product>>
    {
        private readonly IGenericRepository<Product> _repo;

        public GetProductsByCategoryHandler(IGenericRepository<Product> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Product>> Handle(GetProductsByCategory request, CancellationToken ct)
        {
            // CORRECCIÓN: Usar el método FindAsync que sí existe
            return await _repo.FindAsync(p => p.Category == request.Category);
        }
    }
}
