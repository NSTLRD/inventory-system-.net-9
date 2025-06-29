// Application/Commands/DeleteProduct.cs
using System;
using MediatR;

namespace Products.Api.Application.Commands
{
    public class DeleteProduct : IRequest<Unit>
    {
        public Guid Id { get; set; }  // Cambiado de readonly a set para permitir asignación

        public DeleteProduct()
        {
        }

        public DeleteProduct(Guid id)
        {
            Id = id;
        }
    }
}
