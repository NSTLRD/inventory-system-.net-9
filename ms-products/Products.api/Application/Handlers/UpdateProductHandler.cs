// Products.Api/Application/Handlers/UpdateProductHandler.cs
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Products.Api.Application.Commands;
using Products.Api.Common.Interfaces;
using Products.Api.Domain.Entities;
using Products.Api.Domain.Events;
using Products.Api.Infrastructure.Messaging;

namespace Products.Api.Application.Handlers
{
    public class UpdateProductHandler : IRequestHandler<UpdateProduct>
    {
        private readonly IUnitOfWork _uow;
        private readonly KafkaProducer _kafka;

        public UpdateProductHandler(IUnitOfWork uow, KafkaProducer kafka)
        {
            _uow = uow;
            _kafka = kafka;
        }

        public async Task Handle(UpdateProduct request, CancellationToken ct)
        {
            var product = await _uow.Repository<Product>().GetByIdAsync(request.Id);
            if (product == null)
            {
                return;
            }

            // CORRECCIÓN: Usar el método de actualización que hemos creado
            product.Update(
                request.Name,
                request.Description,
                request.Category,
                request.Price,
                request.Sku
            );

            _uow.Repository<Product>().UpdateAsync(product);
            await _uow.SaveChangesAsync(ct);

            var evt = new ProductUpdatedEvent(product.Id, product.Name, product.Description, product.Category, product.Price, product.SKU);
            await _kafka.PublishAsync(evt);
        }
    }
}
