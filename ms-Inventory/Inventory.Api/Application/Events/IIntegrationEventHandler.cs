// Inventory.Api/Application/Events/IIntegrationEventHandler.cs
using System.Threading.Tasks;
using Inventory.Api.Application.Events.Common; // <-- AÑADIR

namespace Inventory.Api.Application.Events
{
    /// <summary>
    /// Handler para un IntegrationEvent específico.
    /// </summary>
    public interface IIntegrationEventHandler<in TEvent>
        where TEvent : IntegrationEvent
    {
        Task Handle(TEvent @event);
    }
}
