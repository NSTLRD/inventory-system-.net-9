
using System;
using MediatR;

namespace Products.Api.Application.Commands
{
    public class DeleteProduct : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public DeleteProduct()
        {
        }

        public DeleteProduct(Guid id)
        {
            Id = id;
        }
    }
}
