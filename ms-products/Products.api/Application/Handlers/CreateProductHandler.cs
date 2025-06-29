// Products.Api/Application/Handlers/CreateProductHandler.cs
using MediatR;
using Products.Api.Application.Commands;
using Products.Api.Common.Interfaces;
using Products.Api.Domain.Entities;
using Products.Api.Domain.Events;
using Products.Api.Infrastructure.Messaging;

namespace Products.Api.Application.Handlers
{
    public class CreateProductHandler : IRequestHandler<CreateProduct, Guid>
    {
        private readonly IUnitOfWork   _uow;
        private readonly KafkaProducer _kafka;

        public CreateProductHandler(IUnitOfWork uow, KafkaProducer kafka)
        {
            _uow   = uow;
            _kafka = kafka;
        }

        public async Task<Guid> Handle(CreateProduct request, CancellationToken ct)
        {
            var id = Guid.NewGuid();
            var product = new Product(
                id,
                request.Name,
                request.Description,
                request.Category,
                request.Price,
                request.Sku);

            await _uow.Repository<Product>().AddAsync(product);
            await _uow.SaveChangesAsync(ct);

            // aquí envío el stock inicial:
            var evt = new ProductCreatedEvent(
                id,
                product.Name,
                product.Description,
                product.Category,
                product.Price,
                product.SKU,
                initialStock: 1);

            await _kafka.PublishAsync(evt);

            return id;
        }
    }
}
