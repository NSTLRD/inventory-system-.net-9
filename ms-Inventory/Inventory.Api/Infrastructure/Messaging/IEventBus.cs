// Inventory.Api/Infrastructure/Messaging/IEventBus.cs
using System.Threading;
using System.Threading.Tasks;
using Inventory.Api.Application.Events.Common; // <-- CORRECCIÓN
using Inventory.Api.Application.Events;

namespace Inventory.Api.Infrastructure.Messaging
{
    /// <summary>
    /// Bus interno para publicar y suscribir IntegrationEvents.
    /// </summary>
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : IntegrationEvent;

    void Subscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>;
}

}
