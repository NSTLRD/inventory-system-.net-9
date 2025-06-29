
using MediatR;

namespace Products.Api.Domain.Events
{
    public record ProductCreated(
        Guid Id,
        string Name,
        string Description,
        string Category,
        decimal Price,
        string SKU
    ) : INotification;

    public record ProductUpdated(
        Guid Id,
        string Name,
        string Description,
        string Category,
        decimal Price,
        string SKU
    ) : INotification;

    public record ProductDeleted(
        Guid Id
    ) : INotification;
}
